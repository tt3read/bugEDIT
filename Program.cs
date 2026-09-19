using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace bugEDIT
{
    internal static class Program
    {
        const string VER = "v1.0.1.0";
        static string CfgPath => Path.Combine(AppContext.BaseDirectory, "bugEDIT.cfg");
        static string adbExe = "adb";
        static string fbExe = "fastboot";

        static void Main()
        {
            Console.Title = "bugEDIT " + VER;
            try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }
            LoadCfg();
            Banner();
            CheckTools();
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("[1] подключить устройство");
                Console.WriteLine("[2] инфо и настройки");
                Console.WriteLine("[3] выход");
                Console.Write("> ");
                var choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1": Connect(); break;
                    case "2": Info(); break;
                    case "3": Console.WriteLine("Пока, друг! 👋"); return;
                    default: Console.WriteLine("Неизвестная команда."); break;
                }
            }
        }

        static void Banner()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  bugEDIT " + VER);
            Console.WriteLine("  консольный инструмент для телефона");
            Console.WriteLine("  (работает через adb / fastboot)");
            Console.WriteLine("========================================");
            Console.WriteLine("Привет мой друг. Это bugEDIT " + VER);
            Console.WriteLine("⚠️ Прошивка/рут отнимают гарантию и могут «окирпить» телефон.");
        }

        // ---- настройки путей ----
        static void LoadCfg()
        {
            try
            {
                if (!File.Exists(CfgPath)) return;
                foreach (var line in File.ReadAllLines(CfgPath))
                {
                    var kv = line.Split('=', 2);
                    if (kv.Length != 2) continue;
                    if (kv[0].Trim() == "adb") adbExe = kv[1].Trim();
                    if (kv[0].Trim() == "fastboot") fbExe = kv[1].Trim();
                }
            }
            catch { }
        }
        static void SaveCfg()
        {
            try { File.WriteAllLines(CfgPath, new[] { "adb=" + adbExe, "fastboot=" + fbExe }); } catch { }
        }

        static string? FindCommon(string tool)
        {
            var dirs = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Android", "Sdk", "platform-tools"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "platform-tools"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "platform-tools"),
                @"C:\platform-tools",
                @"C:\Android\platform-tools"
            };
            foreach (var d in dirs)
            {
                var p = Path.Combine(d, tool + ".exe");
                if (File.Exists(p)) return p;
            }
            return null;
        }

        static void CheckTools()
        {
            var (o1, ok1) = Run2(adbExe, "version");
            if (ok1) return;
            var found = FindCommon("adb");
            if (found != null) { adbExe = found; fbExe = found.Replace("adb.exe", "fastboot.exe"); SaveCfg(); Console.WriteLine("✅ Нашёл adb: " + found); return; }
            Console.WriteLine();
            Console.WriteLine("⚠️ adb не найден! Поставь Android Platform Tools:");
            Console.WriteLine("   https://developer.android.com/tools/releases/platform-tools");
            Console.WriteLine("   Распакуй platform-tools и укажи путь в [2] → [6].");
            Console.WriteLine();
        }

        // [1] подключить устройство
        static void Connect()
        {
            Console.WriteLine("Ищу устройство (adb)…");
            var (adb, ok) = Run2(adbExe, "devices");
            Console.WriteLine(adb);
            if (!ok)
            {
                Console.WriteLine("⚠️ adb не запустился. Укажи путь к platform-tools в [2] → [6].");
                return;
            }
            var lines = adb.Split('\n').Where(l => l.Trim().EndsWith("\tdevice")).ToArray();
            if (lines.Length == 0)
            {
                Console.WriteLine("⚠️ Устройство не найдено. Проверь:");
                Console.WriteLine("  1) Отладка по USB (Настройки → Для разработчиков)");
                Console.WriteLine("  2) Подтверди доступ на экране телефона");
                Console.WriteLine("  3) Кабель/драйверы");
                var (fb, fok) = Run2(fbExe, "devices");
                if (fok && !string.IsNullOrWhiteSpace(fb)) Console.WriteLine("fastboot: " + fb);
                return;
            }
            foreach (var l in lines) Console.WriteLine("✅ Подключено: " + l.Split('\t')[0]);
        }

        // [2] инфо и настройки
        static void Info()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("--- ИНФО И НАСТРОЙКИ ---");
                Console.WriteLine("[1] инфо об устройстве");
                Console.WriteLine("[2] бекап");
                Console.WriteLine("[3] поставить кастомную ОС (прошивка)");
                Console.WriteLine("[4] рут-права (инфо)");
                Console.WriteLine("[5] файлы (pull / push)");
                Console.WriteLine("[6] путь к platform-tools (adb/fastboot)");
                Console.WriteLine("[0] назад");
                Console.Write("> ");
                var c = Console.ReadLine()?.Trim();
                switch (c)
                {
                    case "1": DeviceInfo(); break;
                    case "2": Backup(); break;
                    case "3": Flash(); break;
                    case "4": RootInfo(); break;
                    case "5": Files(); break;
                    case "6": SetPath(); break;
                    case "0": return;
                    default: Console.WriteLine("Неизвестно."); break;
                }
            }
        }

        static void SetPath()
        {
            Console.WriteLine("Укажи ПАПКУ, где лежат adb.exe и fastboot.exe (platform-tools).");
            Console.WriteLine("Пример: C:\\platform-tools");
            Console.Write("Путь: ");
            var dir = Console.ReadLine()?.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(dir)) return;
            var adb = Path.Combine(dir, "adb.exe");
            var fb = Path.Combine(dir, "fastboot.exe");
            if (!File.Exists(adb)) { Console.WriteLine("❌ Не нашёл " + adb); return; }
            adbExe = adb; fbExe = File.Exists(fb) ? fb : "fastboot";
            SaveCfg();
            Console.WriteLine("✅ Сохранено: " + adbExe);
        }

        static void DeviceInfo()
        {
            string[] props = {
                "ro.product.manufacturer", "ro.product.model", "ro.product.device",
                "ro.build.version.release", "ro.build.version.sdk",
                "ro.build.display.id", "ro.serialno"
            };
            foreach (var p in props)
            {
                var (v, _) = Run2(adbExe, $"shell getprop {p}");
                v = v.Trim();
                Console.WriteLine($"  {p} = {(string.IsNullOrEmpty(v) ? "-" : v)}");
            }
        }

        static void Backup()
        {
            Console.WriteLine("Бекап: [1] полный (adb backup)  [2] папка /sdcard (pull)");
            Console.Write("> ");
            var c = Console.ReadLine()?.Trim();
            var name = "backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (c == "1")
            {
                Console.WriteLine("На телефоне подтверди резервное копирование!");
                var (o, ok) = Run2(adbExe, $"backup -apk -shared -all -f \"{name}.ab\"");
                Console.WriteLine(o);
                if (ok && File.Exists(name + ".ab"))
                    Console.WriteLine("✅ Создан файл " + name + ".ab");
                else
                    Console.WriteLine("❌ Бекап НЕ создан (см. ошибку выше).");
            }
            else if (c == "2")
            {
                Directory.CreateDirectory(name);
                var (o, ok) = Run2(adbExe, $"pull /sdcard/ \"{name}/\"");
                Console.WriteLine(o);
                Console.WriteLine(ok ? "✅ Скопировано в папку " + name + "/" : "❌ Не удалось.");
            }
        }

        static void Flash()
        {
            Console.WriteLine("⚠️ ПРОШИВКА: отнимает гарантию и может «окирпить» телефон!");
            Console.WriteLine("Нужны РЕАЛЬНЫЕ файлы: boot.img / recovery.img / ROM.zip");
            Console.WriteLine("Телефон должен быть в fastboot: adb reboot bootloader");
            Console.Write("Путь к .img (Enter — назад): ");
            var img = Console.ReadLine()?.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(img)) return;
            if (!File.Exists(img)) { Console.WriteLine("❌ Файл не найден: " + img); return; }
            Console.WriteLine("[1] fastboot flash boot <img>   [2] fastboot boot <img>   [3] fastboot flash recovery <img>");
            Console.Write("> ");
            var c = Console.ReadLine()?.Trim();
            var args = c == "2" ? $"boot \"{img}\""
                     : c == "3" ? $"flash recovery \"{img}\""
                     : $"flash boot \"{img}\"";
            Console.WriteLine("fastboot " + args);
            var (o, ok) = Run2(fbExe, args);
            Console.WriteLine(o);
            Console.WriteLine(ok ? "✅ Готово (если в выводе нет ошибок)." : "❌ fastboot вернул ошибку.");
        }

        static void RootInfo()
        {
            Console.WriteLine("🔓 РУТ-ПРАВА — что важно знать:");
            Console.WriteLine(" - отнимает гарантию;");
            Console.WriteLine(" - способ: Magisk (патч boot.img) — самый безопасный;");
            Console.WriteLine(" - разблокировка загрузчика: fastboot flashing unlock (СОТРЁТ все данные!);");
            Console.WriteLine(" - после рута банки/DRM/Google Pay могут не работать.");
            Console.WriteLine("Автоматом НЕ рутуем — это опасно, нужна инструкция под конкретную модель.");
        }

        static void Files()
        {
            Console.WriteLine("[1] pull (телефон → ПК)   [2] push (ПК → телефон)");
            Console.Write("> ");
            var c = Console.ReadLine()?.Trim();
            if (c == "1")
            {
                Console.Write("Путь на телефоне (напр. /sdcard/DCIM/): ");
                var p = Console.ReadLine();
                var (o, _) = Run2(adbExe, $"pull \"{p}\" .");
                Console.WriteLine(o);
            }
            else if (c == "2")
            {
                Console.Write("Локальный файл: ");
                var f = Console.ReadLine();
                Console.Write("Куда на телефоне (напр. /sdcard/): ");
                var d = Console.ReadLine();
                var (o, _) = Run2(adbExe, $"push \"{f}\" \"{d}\"");
                Console.WriteLine(o);
            }
        }

        // запуск инструмента; возвращает (вывод, успех по коду возврата)
        static (string output, bool ok) Run2(string exe, string args)
        {
            try
            {
                var psi = new ProcessStartInfo(exe, args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using var p = Process.Start(psi);
                if (p == null) return ("⚠️ Не удалось запустить " + exe, false);
                string o = p.StandardOutput.ReadToEnd();
                string e = p.StandardError.ReadToEnd();
                p.WaitForExit();
                var all = (o + "\n" + e).Trim();
                return (all, p.ExitCode == 0);
            }
            catch (Exception ex)
            {
                return ("⚠️ Не удалось запустить " + exe + ": " + ex.Message +
                        "\n   Установи Android Platform Tools и укажи путь в [2] → [6].", false);
            }
        }
    }
}
