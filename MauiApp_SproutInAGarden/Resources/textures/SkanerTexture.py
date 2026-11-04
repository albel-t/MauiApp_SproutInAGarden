import os
import re
import shutil

SRC_ROOT = r"D:\projects\VisualStudio\MauiApp_SproutInAGarden\MauiApp_SproutInAGarden\Resources\textures"
DST_ROOT = r"D:\projects\VisualStudio\MauiApp_SproutInAGarden\MauiApp_SproutInAGarden\Resources\Raw\textures"

# A_20_9_21_109.png → letter="A", numbers=[20, 9, 21, 109], ext="png"
NAME_RE = re.compile(r"^([A-Za-z])_(\d+)_(\d+)_(\d+)_(\d+)\.(\w+)$")


def convert(src_root: str, dst_root: str) -> None:
    for dirpath, dirnames, filenames in os.walk(src_root):
        if not filenames:
            continue

        rel = os.path.relpath(dirpath, src_root)
        dst_dir = os.path.join(dst_root, rel) if rel != "." else dst_root

        for fname in filenames:
            m = NAME_RE.match(fname)
            if not m:
                print(f"[skip] не подходит: {os.path.join(dirpath, fname)}")
                continue

            letter = m.group(1)
            numbers = [m.group(2), m.group(3), m.group(4), m.group(5)]
            ext = m.group(6)

            letter_dir = os.path.join(dst_dir, letter)
            os.makedirs(letter_dir, exist_ok=True)

            # 1) картинка: <letter>.<ext>
            src_file = os.path.join(dirpath, fname)
            dst_img = os.path.join(letter_dir, f"{letter}.{ext}")
            shutil.copy2(src_file, dst_img)

            # 2) данные прилегания: <letter>_data.txt, числа через пробел
            dst_data = os.path.join(letter_dir, f"{letter}_data.txt")
            with open(dst_data, "w", encoding="utf-8") as f:
                f.write(" ".join(numbers))

            print(f"{src_file}  →  {dst_img}  +  {dst_data} ({' '.join(numbers)})")


def write_manifest(dst_root: str, manifest_path: str) -> None:
    lines = []
    for dirpath, dirnames, filenames in os.walk(dst_root):
        for fname in filenames:
            rel = os.path.relpath(os.path.join(dirpath, fname), dst_root)
            lines.append(rel.replace("\\", "/"))

    with open(manifest_path, "w", encoding="utf-8") as f:
        f.write("\n".join(sorted(lines)))

    print(f"manifest: {manifest_path} ({len(lines)} файлов)")


if __name__ == "__main__":
    if os.path.exists(DST_ROOT):
        shutil.rmtree(DST_ROOT)
    convert(SRC_ROOT, DST_ROOT)
    write_manifest(DST_ROOT, os.path.join(DST_ROOT, "manifest.txt"))