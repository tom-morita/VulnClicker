import base64
import hashlib
import argparse


def encode(text: str) -> str:
    data = text.encode("utf-8")
    return base64.b64encode(data).decode("ascii")


def decode(text: str) -> str:
    data = base64.b64decode(text, validate=True)
    return data.decode("utf-8")


def calc_sha256_series(text: str) -> str:
    data = ""
    items = []
    items = text.split(":")
    for i in range(len(items)):
        text = ":".join(items[:i+1])
        data += "\nSHA256 [0:" + str(i+1) + "]: " + text +"\n" + hashlib.sha256(text.encode("utf-8")).hexdigest()
    return data


def main():
    parser = argparse.ArgumentParser(
        description="Base64 Encoder / Decoder + SHA256 Hash Calculator"
    )

    parser.add_argument(
        "mode",
        choices=["encode", "decode"],
        help="encode または decode"
    )

    parser.add_argument(
        "text",
        help="変換する文字列"
    )

    args = parser.parse_args()

    try:
        if args.mode == "encode":
            result = "========= Input =========\n" + args.text
            result += "\n\n========= Encode =========\n" + encode(args.text)
            result += "\n\n===== SHA256 series =====" + calc_sha256_series(args.text)
        else:
            result = "========= Input =========\n" + args.text
            result += "\n\n========= Decode =========\n" + decode(args.text)
            result += "\n\n===== SHA256 series =====" + calc_sha256_series(decode(args.text))

        print(result)

    except Exception as e:
        print(f"Error: {e}")


if __name__ == "__main__":
    main()