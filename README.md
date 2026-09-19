#bugEDIT

**Консольный инструмент для прошивки и обслуживания Android-телефонов через `adb` / `fastboot`.**

Ставит кастомную ОС, делает бекапы, показывает инфо, работает с файлами.
Написан на **C# / .NET 8**.

[![build](https://github.com/tt3read/bugEDIT/actions/workflows/build.yml/badge.svg)](https://github.com/tt3read/bugEDIT/actions/workflows/build.yml)

> ⚠️ **ВНИМАНИЕ:** прошивка и рут **отнимают гарантию** и могут «окирпить» телефон.
> Делай только на **СВОЁМ** устройстве и на свой риск.

---

## 📟 Меню

```
========================================
  bugEDIT v1.0.1.0
  консольный инструмент для телефона
  (работает через adb / fastboot)
========================================
Привет мой друг. Это bugEDIT v1.0.1.0
⚠️ Прошивка/рут отнимают гарантию и могут «окирпить» телефон.

[1] подключить устройство
[2] инфо и настройки
[3] выход
```

Раздел **[2] Инфо и настройки**:

```
[1] инфо об устройстве
[2] бекап
[3] поставить кастомную ОС (прошивка)
[4] рут-права (инфо)
[5] файлы (pull / push)
[6] путь к platform-tools (adb/fastboot)
[0] назад
```

---

## ✨ Возможности

- 🔌 **Подключение устройства** — `adb devices`, проверка связи
- 📱 **Инфо об устройстве** — модель, производитель, версия Android, SDK, серийник
- 💾 **Бекап** — полный (`adb backup … .ab`) или папка `/sdcard` (`adb pull`)
- 💿 **Кастомная ОС** — `fastboot flash boot/recovery <img>`
- 🔓 **Рут-права** — инфо про Magisk + предупреждения
- 📁 **Файлы** — `pull` / `push`
- ⚙️ **Путь к Platform Tools** — сохраняется в `bugEDIT.cfg`

## 📂 Реальные форматы файлов
- `*.img` — boot / recovery / system (прошивка)
- `*.zip` — ROM (например, `adb sideload rom.zip`)
- `*.ab` — бекап (adb backup)
- `*.apk` — приложения

---

## 🧰 Что нужно

1. **Windows** + **.NET 8** (SDK — для сборки, или готовый `.exe`)
2. **Android Platform Tools** (в них `adb` и `fastboot`):
   https://developer.android.com/tools/releases/platform-tools
3. На телефоне — **Отладка по USB** (Настройки → Для разработчиков)

> Если `adb` не находится — в программе есть пункт **[2] → [6]**: укажи папку с
> `platform-tools`, путь сохранится. (Или добавь папку в `PATH`.)

---

## 🛠 Сборка

```bash
dotnet build -c Release
```

Один файл `.exe`:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
# → bin/Release/net8.0/win-x64/publish/bugEDIT.exe
```

Готовый `.exe` также собирается автоматически в **GitHub Actions** (вкладка *Actions* → *Artifacts*).

---

## 🚀 Запуск

```bash
bugEDIT.exe
```

или

```bash
dotnet bugEDIT.dll
```

---

## 🗺 Планы

- [ ] `adb sideload` для ROM.zip
- [ ] автопоиск прошивок
- [ ] сохранение истории операций

---

## 📜 Лицензия

MIT — делай что хочешь, но на свой риск.
