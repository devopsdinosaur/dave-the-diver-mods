
import os
import sys
import shutil

IN_DIR = "C:/tmp/decompiled/dave-the-diver"
OUT_DIR = IN_DIR
REMOVE_SEALED_CLASSES = True

def remove_annotations(in_root, out_root, offset):
    if (offset):
        check_directory = os.path.join(in_root, offset)
        out_directory = os.path.join(out_root, offset)
    else:
        check_directory = in_root
        out_directory = out_root
    if (not os.path.exists(out_directory)):
        os.makedirs(out_directory)
    for file in os.listdir(check_directory):
        new_offset = os.path.join(offset, file) if (offset) else file
        in_path = os.path.join(check_directory, file)
        if (os.path.isdir(in_path)):
            remove_annotations(in_root, out_root, new_offset)
            continue
        out_path = os.path.join(out_directory, file)
        if (os.path.splitext(file)[1] != ".cs"):
            try:
                shutil.copyfile(in_path, out_path)
            except:
                pass
            continue
        print(in_path)
        f = open(in_path, "r")
        data = f.read()
        f.close()
        new_lines = []
        sealed_class_depth = 0
        lines = data.split("\n")
        line_index = 0
        while (line_index < len(lines)):
            line = lines[line_index]
            check_line = line.strip()
            if (len(check_line) > 2 and check_line[0] == '[' and check_line[-1] == ']'):
                line_index += 1
                continue
            if (sealed_class_depth == 0):
                if (check_line.startswith("private sealed class")):
                    sealed_class_depth = 1
                    while ('{' not in lines[line_index]):
                        line_index += 1
                    line_index += 1
                    continue
                if (check_line != "" or new_lines == [] or new_lines[-1].strip() != ""):
                    new_lines.append(line)
            else: 
                if ('}' in check_line):
                    sealed_class_depth -= 1
                if ('{' in check_line):
                    sealed_class_depth += 1
            line_index += 1
        f = open(out_path, "w")
        f.write("\n".join(new_lines))
        f.close()
    return 0

if (__name__ == "__main__"):
    sys.exit(remove_annotations(IN_DIR, OUT_DIR, None))