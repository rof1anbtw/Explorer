using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer
{
    class Program
    {
        static string path;
        static string[] files, dirs;
        static int prevpathlen = 20;
        static List<string> prevPath = new List<string>(prevpathlen + 1);
        static void openPath(string p, bool prev = true)
        {
            if (p != "" && prev)
            {
                if (prevPath.Count == prevpathlen)
                    prevPath.RemoveAt(0);
                prevPath.Add(p);
            }
            if (p == "")
            {
                Console.Clear();
                path = menu(System.IO.Directory.GetLogicalDrives(), "Проводник:");
                if (prev)
                {
                    if (prevPath.Count == prevpathlen)
                        prevPath.RemoveAt(0);
                    prevPath.Add(path);
                }
                files = correctArrey(System.IO.Directory.GetFiles(path));
                dirs = correctArrey(System.IO.Directory.GetDirectories(path));
            }
            else
            {
                path = p;
                while (path[path.Length - 1] == '\\')
                    path = path.Remove(path.Length - 1);
                if (path.Length < 3) path += '\\';
            }
        }

        static void Main(string[] args)
        {
            if (args.Length > 0 && System.IO.Directory.Exists(args[0]))
                openPath(args[0]);
            else
                openPath("");
            while (true)
            {
                int selected = 0, last = 0, totalLength = dirs.Length + files.Length;
                bool rewrite = false;
                Console.Title = path;
                Console.Clear();
                Console.WriteLine("папки :\n");
                for (int i = 0; i < dirs.Length; i++)
                    Console.WriteLine((i == selected ? "      --->: " : "папка  : ") + dirs[i]);
                Console.WriteLine("\nфайлы :\n");
                for (int i = 0; i < files.Length; i++)
                    Console.WriteLine((selected >= dirs.Length && i == selected - dirs.Length ? "      ---> : " : "файл : ") + files[i]);
                while (true)
                {
                    ConsoleKeyInfo ck = Console.ReadKey(true);
                    if (ck.Key == ConsoleKey.DownArrow)
                    {
                        selected++;
                        if (selected > totalLength - 1)
                            selected = 0;
                    }
                    else if (ck.Key == ConsoleKey.UpArrow)
                    {
                        selected--;
                        if (selected < 0)
                            selected = totalLength - 1;
                    }
                    else if (ck.Key == ConsoleKey.End)
                    {
                        selected = (selected < dirs.Length) ? dirs.Length - 1 : dirs.Length + files.Length - 1;
                    }
                    else if (ck.Key == ConsoleKey.Home)
                    {
                        selected = (selected < dirs.Length) ? 0 : (files.Length == 0) ? 0 : dirs.Length;
                    }
                    else if (ck.Key == ConsoleKey.Enter)
                    {
                        if (selected < dirs.Length)
                        {
                            openPath((path.Length > 3) ? path + '\\' + dirs[selected] : path + dirs[selected]);
                            break;
                        }
                        else
                            try
                            {
                                System.Diagnostics.Process.Start(path + '\\' + files[selected - dirs.Length]);
                            }
                            catch { }
                    }
                    else if (ck.Key == ConsoleKey.Backspace)
                    {
                        if (path.Length > 3)
                        {
                            openPath(cutLast(path));
                            break;
                        }
                        else
                        {
                            Console.Clear();
                            openPath("");
                            break;
                        }
                    }
                    else if (ck.Key == ConsoleKey.Tab)
                    {
                        bool ex = false;
                        string op1 = (selected < dirs.Length) ? "OОткрыть выбранное в проводникеr" : "Run width argument";
                        string mes = menu(new string[] { "Назад", "Перейти", "домой", "Info (путь)", "info (выборанный файл/папка)", "Удалить", "Копировать", op1, "открыть путь в проводнике", "Предыдущий", "Выход" }, "select an item then press enter\n");
                        {
                            if (mes == "Назад")
                                ex = true;
                            else if (mes == "Перейти")
                            {
                                Console.Clear();
                                ex = true;
                                Console.Write("введите путь для перехода: ");
                                string p = Console.ReadLine();
                                while (p.Length > 1 && !System.IO.Directory.Exists(p))
                                {
                                    Console.Write("введите правильный путь : ");
                                    p = Console.ReadLine();
                                }
                                openPath(p);
                            }
                            else if (mes == "домой")
                            {
                                ex = true;
                                openPath("");
                            }
                            else if (mes == "Info (путь)")
                            {
                                Console.Clear();
                                ex = true;
                                Console.WriteLine(path);
                                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path);
                                Console.WriteLine("Дата создания : " + di.CreationTime.ToLongDateString() + "  " + di.CreationTime.ToLongTimeString());
                                Console.WriteLine("Дата последнего открытия : " + di.LastAccessTime.ToLongDateString() + "  " + di.LastAccessTime.ToLongTimeString());
                                Console.WriteLine("Дата последнего изменения : " + di.LastWriteTime.ToLongDateString() + "  " + di.LastWriteTime.ToLongTimeString());
                                Console.WriteLine(di.Attributes.ToString());
                                Console.Write("нажмите любую кнопку чтобы продолжить");
                                Console.ReadKey(true);
                            }
                            else if (mes == "info (выбор)")
                            {
                                Console.Clear();
                                ex = true;
                                if (selected < dirs.Length)
                                {
                                    Console.WriteLine(path + "\\" + dirs[selected]);
                                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(path + "\\" + dirs[selected]);
                                    Console.WriteLine("Дата создания папки : " + di.CreationTime.ToLongDateString() + "  " + di.CreationTime.ToLongTimeString());
                                    Console.WriteLine("Дата последнего открытия папки : " + di.LastAccessTime.ToLongDateString() + "  " + di.LastAccessTime.ToLongTimeString());
                                    Console.WriteLine("Дата последнего изменения папки : " + di.LastWriteTime.ToLongDateString() + "  " + di.LastWriteTime.ToLongTimeString());
                                    Console.WriteLine(di.Attributes.ToString());
                                    Console.Write("нажмите любую кнопку чтобы продолжить");
                                    Console.ReadKey(true);
                                }
                                else
                                {

                                    Console.WriteLine(path + "\\" + files[selected - dirs.Length]);
                                    System.IO.FileInfo di = new System.IO.FileInfo(path + "\\" + files[selected - dirs.Length]);
                                    float gbFileSize = (float)(di.Length / 1024.0 / 1024.0);
                                    Console.WriteLine("размер файла : " + di.Length.ToString() + " Bt            " + ((gbFileSize >= 921.6) ? "(" + (gbFileSize / 1024.0).ToString() + " GB)" : ((gbFileSize >= 0.9216) ? "(" + (gbFileSize).ToString() + " MB)" : "(" + (gbFileSize * 1024.0).ToString() + " KB)")));
                                    Console.WriteLine("Дата создания файла : " + di.CreationTime.ToLongDateString() + "  " + di.CreationTime.ToLongTimeString());
                                    Console.WriteLine("Дата последнего открытия файла : " + di.LastAccessTime.ToLongDateString() + "  " + di.LastAccessTime.ToLongTimeString());
                                    Console.WriteLine("Дата последнего изменения файла : " + di.LastWriteTime.ToLongDateString() + "  " + di.LastWriteTime.ToLongTimeString());
                                    Console.WriteLine(di.Attributes.ToString());
                                    Console.Write("нажмите любую кнопку чтобы продолжить");
                                    Console.ReadKey(true);
                                }
                            }


                            else if (selected >= dirs.Length && mes == "Копировать")
                            {
                                Console.Clear();
                                ex = true;
                                Console.Write("Введите путь для перемещения: ");
                                string path2 = Console.ReadLine();
                                try
                                {
                                    System.IO.File.Copy(path + "\\" + files[selected - dirs.Length], path2);
                                }
                                catch (Exception exe) { Console.Write(exe.Message); }
                                System.Threading.Thread.Sleep(1000);
                            }
                            else if (mes == op1)
                            {
                                if (selected < dirs.Length)
                                {
                                    ex = true;
                                    System.Diagnostics.Process.Start(path + '\\' + dirs[selected]);
                                }
                                else
                                {
                                    Console.Clear();
                                    ex = true;
                                    Console.Write("введите аргумент : ");
                                    System.Diagnostics.Process.Start(path + '\\' + files[selected - dirs.Length], Console.ReadLine());
                                }
                            }
                            else if (mes == "открыть путь в проводнике")
                            {
                                ex = true;
                                System.Diagnostics.Process.Start(path + '\\' + dirs[selected]);
                            }
                            else if (mes == "Предыдущий")
                            {
                                ex = true;
                                prevPath.Insert(0, "Назад");
                                int innerMes = menu2(prevPath.ToArray(), "выберите элемент, чтобы перейти к нему\n") - 1;
                                prevPath.RemoveAt(0);
                                if (innerMes != -1)
                                {
                                    string p = prevPath[innerMes];
                                    prevPath.RemoveAt(innerMes);
                                    openPath(p);
                                }
                            }
                            else if (mes == "Выход")
                                System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                        if (ex)
                            break;
                    }
                    else
                    {
                        if (selected < dirs.Length)
                        {
                            for (int n = (selected < dirs.Length - 1) ? selected + 1 : 0; n < dirs.Length; n++)
                            {
                                if (Char.ToLower(dirs[n][0]) == Char.ToLower(ck.KeyChar))
                                {
                                    selected = n;
                                    break;
                                }
                                else if (n == dirs.Length - 1)
                                {
                                    for (int m = 0; m < dirs.Length; m++)
                                        if (Char.ToLower(dirs[m][0]) == Char.ToLower(ck.KeyChar))
                                        {
                                            selected = m;
                                            break;
                                        }
                                }

                            }
                        }
                        else
                        {

                            for (int n = (selected - dirs.Length < files.Length - 1) ? selected - dirs.Length + 1 : dirs.Length; n < files.Length; n++)
                            {
                                if (Char.ToLower(files[n][0]) == Char.ToLower(ck.KeyChar))
                                {
                                    selected = dirs.Length + n;
                                    break;
                                }
                                else if (n == files.Length - 1)
                                {
                                    for (int m = 0; m < files.Length; m++)
                                        if (Char.ToLower(files[m][0]) == Char.ToLower(ck.KeyChar))
                                        {
                                            selected = m + dirs.Length;
                                            break;
                                        }
                                }
                            }
                        }
                    }
                    if (last == selected && !rewrite)
                    {
                        continue;
                    }
                    if (selected < dirs.Length)
                    {
                        if (last < dirs.Length)
                        {
                            Console.SetCursorPosition(0, last + 2);
                            Console.WriteLine(emptyString(dirs[last].Length + 21));
                            Console.SetCursorPosition(0, last + 2);
                            Console.Write("папка  : " + dirs[last]);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, last + 5);
                            Console.WriteLine(emptyString(files[last - dirs.Length].Length + 21));
                            Console.SetCursorPosition(0, last + 5);
                            Console.Write("файл : " + files[last - dirs.Length]);
                        }
                        Console.SetCursorPosition(0, selected + 2);
                        Console.Write("      ---> : " + dirs[selected]);
                    }
                    else
                    {
                        if (last < dirs.Length)
                        {
                            Console.SetCursorPosition(0, last + 2);
                            Console.WriteLine(emptyString(dirs[last].Length + 21));
                            Console.SetCursorPosition(0, last + 2);
                            Console.Write("папка  : " + dirs[last]);
                        }
                        else
                        {
                            Console.SetCursorPosition(0, last + 5);
                            Console.WriteLine(emptyString(files[last - dirs.Length].Length + 21));
                            Console.SetCursorPosition(0, last + 5);
                            Console.Write("файл : " + files[last - dirs.Length]);
                        }
                        Console.SetCursorPosition(0, selected + 5);
                        Console.Write("   --> : " + files[selected - dirs.Length]);
                    }
                    last = selected;
                }

            }
        }

        static string cutLast(string v)
        {
            int i = v.Length - 1;
            while (v[i] != '\\')
                i--;
            if (i > 0)
                return v.Substring(0, i);
            return v;
        }
        static string[] correctArrey(string[] v)
        {
            for (int i = 0; i < v.Length; i++)
                v[i] = System.IO.Path.GetFileName(v[i]);
            return v;
        }
        static string emptyString(int len)
        {
            string s = "";
            for (int i = 0; i < len; i++)
                s += " ";
            return s;
        }
        static int entersCount(string v)
        {
            int i = 0;
            foreach (char x in v)
                if (x == '\n')
                    i++;
            return i;
        }
        static string menu(string[] items, string title)
        {
            Console.Clear();
            Console.WriteLine(title);
            int enterCount = entersCount(title) + 1;
            int selected = 0, last = 0;
            for (int i = 0; i < items.Length; i++)
                Console.WriteLine((i == selected ? " -->  " : "") + items[i]);
            while (true)
            {
                ConsoleKeyInfo ck = Console.ReadKey(true);
                if (ck.Key == ConsoleKey.DownArrow)
                {
                    selected++;
                    if (selected > items.Length - 1)
                        selected = 0;
                }
                else if (ck.Key == ConsoleKey.UpArrow)
                {
                    selected--;
                    if (selected < 0)
                        selected = items.Length - 1;
                }
                else if (ck.Key == ConsoleKey.Enter)
                {
                    if (selected == -1)
                        return "";
                    else return items[selected];
                }
                if (selected != last)
                {
                    Console.SetCursorPosition(0, enterCount + last);
                    Console.Write(emptyString(12 + items[last].Length));
                    Console.SetCursorPosition(0, enterCount + last);
                    Console.Write(items[last]);
                    Console.SetCursorPosition(0, enterCount + selected);
                    Console.Write("  -->  " + items[selected]);
                }
                last = selected;
            }
        }
        static int menu2(string[] items, string title)
        {
            Console.Clear();
            Console.WriteLine(title);
            int enterCount = entersCount(title) + 1;
            int selected = 0, last = 0;
            for (int i = 0; i < items.Length; i++)
            {
                Console.WriteLine((i == selected ? " -->  " : "") + items[i]);
            }
            while (true)
            {
                ConsoleKeyInfo ck = Console.ReadKey(true);
                if (ck.Key == ConsoleKey.DownArrow)
                {
                    selected++;
                    if (selected > items.Length - 1)
                        selected = 0;
                }
                else if (ck.Key == ConsoleKey.UpArrow)
                {
                    selected--;
                    if (selected < 0)
                        selected = items.Length - 1;
                }
                else if (ck.Key == ConsoleKey.Enter)
                {
                    return selected;
                }
                if (selected != last)
                {
                    Console.SetCursorPosition(0, enterCount + last);
                    Console.Write(emptyString(12 + items[last].Length));
                    Console.SetCursorPosition(0, enterCount + last);
                    Console.Write(items[last]);
                    Console.SetCursorPosition(0, enterCount + selected);
                    Console.Write(" -->  " + items[selected]);
                }
                last = selected;
            }
        }
    }
}