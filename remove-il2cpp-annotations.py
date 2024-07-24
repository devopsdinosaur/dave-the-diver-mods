
import os
import sys

THIS_DIR = os.path.dirname(__file__)
ROOT_DIR = os.path.join(THIS_DIR, "decompiled", "dave-the-diver")

def remove_annotations(directory):
    for file in os.listdir(directory):
        full_path = os.path.join(directory, file)
        if (os.path.isdir(full_path)):
            remove_annotations(full_path)
            continue
        if (os.path.splitext(file)[1] != ".cs"):
            continue
        f = open(full_path, "r")
        data = f.read()
        f.close()
        new_lines = []
        for line in data.split("\n"):
            check_line = line.strip()
            if (len(check_line) > 2 and check_line[0] == '[' and check_line[-1] == ']'):
                continue
            new_lines.append(line)
        f = open(full_path, "w")
        f.write("\n".join(new_lines))
        f.close()
    return 0

if (__name__ == "__main__"):
    sys.exit(remove_annotations(ROOT_DIR))