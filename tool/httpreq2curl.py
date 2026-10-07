#!/usr/bin/env python3

import argparse
import shlex
import sys
from pathlib import Path
from urllib.parse import urlsplit


# HTTPクライアント側に自動生成させるヘッダー
SKIP_HEADERS = {
    "host",
    "content-length",
    "connection",
}


# =========================================================
# HTTP Request解析
# =========================================================

def split_http_request(raw: str) -> tuple[str, str]:
    """
    HTTPリクエストをヘッダー部とボディ部に分割する。
    CRLF / LF の両方に対応。
    """
    normalized = raw.replace("\r\n", "\n")

    if "\n\n" in normalized:
        header_part, body = normalized.split("\n\n", 1)
    else:
        header_part = normalized
        body = ""

    return header_part, body


def parse_http_request(raw: str) -> dict:
    """
    生のHTTP/1.xリクエストを解析する。

    例:

        POST /api/scores HTTP/1.1
        Host: localhost:8080
        Content-Type: application/json

        {"player":"test"}
    """
    header_part, body = split_http_request(raw)

    lines = header_part.splitlines()

    if not lines:
        raise ValueError("HTTPリクエストが空です。")

    request_line = lines[0].strip()

    try:
        method, target, version = request_line.split(None, 2)

    except ValueError:
        raise ValueError(
            f"不正なHTTPリクエストラインです: {request_line}"
        )

    if not version.upper().startswith("HTTP/"):
        raise ValueError(
            f"HTTPバージョンを認識できません: {version}"
        )

    headers = []

    current_name = None
    current_value = None

    for line in lines[1:]:

        # Folded Header
        if line.startswith((" ", "\t")) and current_name is not None:
            current_value += " " + line.strip()
            continue

        # 直前のヘッダーを登録
        if current_name is not None:
            headers.append(
                (current_name, current_value)
            )

        if ":" not in line:
            current_name = None
            current_value = None
            continue

        name, value = line.split(":", 1)

        current_name = name.strip()
        current_value = value.strip()

    # 最後のヘッダー
    if current_name is not None:
        headers.append(
            (current_name, current_value)
        )

    return {
        "method": method.upper(),
        "target": target,
        "version": version,
        "headers": headers,
        "body": body,
    }


def find_header(headers, name: str):
    """
    大文字小文字を無視してHTTPヘッダーを検索する。
    """
    target_name = name.lower()

    for header_name, value in headers:
        if header_name.lower() == target_name:
            return value

    return None


def build_url(request: dict, scheme: str) -> str:
    """
    Request-URI と Host ヘッダーからURLを生成する。
    """
    target = request["target"]
    headers = request["headers"]

    # absolute-form
    #
    # GET http://example.com/test HTTP/1.1
    #
    parsed = urlsplit(target)

    if parsed.scheme in ("http", "https"):
        return target

    host = find_header(
        headers,
        "Host",
    )

    if not host:
        raise ValueError(
            "Hostヘッダーがありません。"
        )

    if not target.startswith("/"):
        target = "/" + target

    return f"{scheme}://{host}{target}"


# =========================================================
# Quote
# =========================================================

def bash_quote(value: str) -> str:
    """
    Bash / POSIX Shell用。
    """
    return shlex.quote(value)


def powershell_quote(value: str) -> str:
    """
    PowerShell用。

    PowerShellのシングルクォート文字列内では
    ' を '' としてエスケープする。
    """
    return "'" + value.replace("'", "''") + "'"


def cmd_quote(value: str) -> str:
    """
    cmd.exe用。

    curl.exeへ渡すための簡易quote処理。
    JSON等に含まれる " は \" とする。
    """
    escaped = value.replace('"', '\\"')

    return f'"{escaped}"'


# =========================================================
# Bash curl
# =========================================================

def create_bash_curl(
    request: dict,
    scheme: str = "http",
    include_all_headers: bool = False,
) -> str:

    url = build_url(
        request,
        scheme,
    )

    method = request["method"]
    headers = request["headers"]
    body = request["body"]

    parts = [
        "curl",
        "-i",
        "-X",
        method,
        bash_quote(url),
    ]

    for name, value in headers:

        lower_name = name.lower()

        if (
            not include_all_headers
            and lower_name in SKIP_HEADERS
        ):
            continue

        parts.extend([
            "-H",
            bash_quote(
                f"{name}: {value}"
            ),
        ])

    if body:
        parts.extend([
            "--data-raw",
            bash_quote(body),
        ])

    return " ".join(parts)


# =========================================================
# Windows cmd.exe curl.exe
# =========================================================

def create_cmd_curl(
    request: dict,
    scheme: str = "http",
    include_all_headers: bool = False,
) -> str:
    """
    cmd.exe用のcurl.exeコマンドを生成する。

    必ず1行で出力する。
    """

    url = build_url(
        request,
        scheme,
    )

    method = request["method"]
    headers = request["headers"]
    body = request["body"]

    parts = [
        "curl.exe",
        "-i",
        "-X",
        method,
        cmd_quote(url),
    ]

    for name, value in headers:

        lower_name = name.lower()

        if (
            not include_all_headers
            and lower_name in SKIP_HEADERS
        ):
            continue

        parts.extend([
            "-H",
            cmd_quote(
                f"{name}: {value}"
            ),
        ])

    if body:
        parts.extend([
            "--data-raw",
            cmd_quote(body),
        ])

    return " ".join(parts)


# =========================================================
# PowerShell Invoke-WebRequest
# =========================================================

def create_invoke_webrequest(
    request: dict,
    scheme: str = "http",
    include_all_headers: bool = False,
) -> str:

    url = build_url(
        request,
        scheme,
    )

    method = request["method"]
    headers = request["headers"]
    body = request["body"]

    normal_headers = []
    content_type = None

    for name, value in headers:

        lower_name = name.lower()

        if (
            not include_all_headers
            and lower_name in SKIP_HEADERS
        ):
            continue

        # Content-Typeは専用パラメーターを使用
        if lower_name == "content-type":
            content_type = value
            continue

        normal_headers.append(
            (name, value)
        )

    lines = [
        "Invoke-WebRequest `",
        f"    -Uri {powershell_quote(url)} `",
        f"    -Method {powershell_quote(method)}",
    ]

    # Headers
    if normal_headers:

        lines[-1] += " `"

        lines.append(
            "    -Headers @{"
        )

        # セミコロンを必ず付ける
        for name, value in normal_headers:

            lines.append(
                "        "
                f"{powershell_quote(name)} = "
                f"{powershell_quote(value)};"
            )

        lines.append(
            "    }"
        )

    # Content-Type
    if content_type:

        lines[-1] += " `"

        lines.append(
            "    -ContentType "
            f"{powershell_quote(content_type)}"
        )

    # Body
    if body:

        lines[-1] += " `"

        lines.append(
            "    -Body "
            f"{powershell_quote(body)}"
        )

    return "\n".join(lines)


# =========================================================
# HTTP Request読み込み
# =========================================================

def read_request(
    filename: str | None,
) -> str:

    if filename:

        path = Path(filename)

        if not path.exists():
            raise FileNotFoundError(
                f"ファイルが見つかりません: {filename}"
            )

        return path.read_text(
            encoding="utf-8",
            errors="replace",
        )

    return sys.stdin.read()


# =========================================================
# main
# =========================================================

def main():

    parser = argparse.ArgumentParser(
        description=(
            "生のHTTP/1.xリクエストから "
            "Bash curl / Windows curl.exe / "
            "Invoke-WebRequest を生成します。"
        )
    )

    parser.add_argument(
        "file",
        nargs="?",
        help=(
            "HTTPリクエストを保存した"
            "テキストファイル"
        ),
    )

    parser.add_argument(
        "--https",
        action="store_true",
        help="URLをHTTPSとして生成する",
    )

    parser.add_argument(
        "--all-headers",
        action="store_true",
        help=(
            "Host / Content-Length / Connection "
            "なども出力する"
        ),
    )

    args = parser.parse_args()

    try:

        raw = read_request(
            args.file
        )

        if not raw.strip():
            raise ValueError(
                "HTTPリクエストが入力されていません。"
            )

        request = parse_http_request(
            raw
        )

        scheme = (
            "https"
            if args.https
            else "http"
        )

        # ---------------------------------------------
        # Bash curl
        # ---------------------------------------------

        bash_curl = create_bash_curl(
            request,
            scheme=scheme,
            include_all_headers=args.all_headers,
        )

        # ---------------------------------------------
        # cmd.exe curl.exe
        # ---------------------------------------------

        cmd_curl = create_cmd_curl(
            request,
            scheme=scheme,
            include_all_headers=args.all_headers,
        )

        # ---------------------------------------------
        # PowerShell Invoke-WebRequest
        # ---------------------------------------------

        invoke_command = create_invoke_webrequest(
            request,
            scheme=scheme,
            include_all_headers=args.all_headers,
        )

        # ---------------------------------------------
        # 出力
        # ---------------------------------------------

        print()
        print("=" * 70)
        print("Bash - curl")
        print("=" * 70)
        print()

        print(bash_curl)

        print()
        print()

        print("=" * 70)
        print("Windows cmd.exe - curl.exe")
        print("=" * 70)
        print()

        # 必ず1行
        print(cmd_curl)

        print()
        print()

        print("=" * 70)
        print("PowerShell - Invoke-WebRequest")
        print("=" * 70)
        print()

        print(invoke_command)

        print()

    except (
        ValueError,
        FileNotFoundError,
    ) as e:

        print(
            f"エラー: {e}",
            file=sys.stderr,
        )

        sys.exit(1)


if __name__ == "__main__":
    main()