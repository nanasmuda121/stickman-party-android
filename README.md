# 📱 Stickman Party Clone (Android 2D Local Multiplayer)

Game pesta 1–4 pemain lokal bergaya **Stickman Party** yang dirancang khusus untuk perangkat **Android (HP / Tablet)** dalam 1 layar bersama, menggunakan arsitektur Unity (C#) dan siap di-build otomatis menjadi file `.apk` via **GitHub Actions**.

---

## 📱 Sistem Kontrol Layar Sentuh 4 Sudut (Multi-Touch di 1 Layar HP/Tablet)

Sama persis seperti game *Stickman Party* aslinya, semua pemain bermain di **1 layar HP atau Tablet yang sama**. Setiap sudut layar memiliki **Virtual Joystick & Tombol Aksi** sesuai warna pemain:

```
┌───────────────────────────────────────────────┐
│ [P4 KUNING]                       [P2 BIRU]   │
│ 🕹️ Joy (Kiri-Atas)            (Kanan-Atas) 🕹️ │
│ 🔘 Action                     Action 🔘       │
│                                               │
│                   ARENA                       │
│                                               │
│ 🔘 Action                     Action 🔘       │
│ 🕹️ Joy (Kiri-Bawah)          (Kanan-Bawah) 🕹️ │
│ [P1 MERAH]                        [P3 HIJAU]  │
└───────────────────────────────────────────────┘
```

* *Catatan Pengujian:* Di Unity Editor (PC), kontrol tetap bisa diuji menggunakan mouse (klik & drag joystick) atau menggunakan tombol keyboard:
  * P1: `WASD` + `Space`
  * P2: `Panah` + `Enter`
  * P3: `IJKL` + `U`
  * P4: `Numpad 8456` + `Numpad +`

---

## 🌟 Fitur Utama

1. **1–4 Pemain Lokal + Bot AI Pintar**
   * Pilih mode 1P, 2P, 3P, atau 4P di layar utama.
   * Slot pemain yang kosong otomatis diisi oleh **Bot AI** yang lincah (misal: Anda sendiri melawan 3 Bot).
2. **Zero External Assets (Langsung Jalan Tanpa Download Berat)**
   * Karakter stickman lentur (*squash & stretch*), mata ekspresif yang menoleh, sarung tinju, dan arena digambar secara prosedural.
   * Efek suara arcade disintesis langsung lewat C# via Unity Audio API (bebas dependensi file `.mp3` eksternal).
3. **Turnamen 5 Ronde & Podium Juara**
   * Putaran 5 mini-game secara acak dengan perolehan bintang (+3 Bintang per kemenangan).
   * Layar podium juara di akhir turnamen.
4. **Auto-Bootstrap System**
   * Cukup buka scene kosong di Unity dan tekan **Play ▶**, game langsung berjalan otomatis!

---

## 🕹️ 5 Mini-Game Seru yang Tersedia

1. **Lava Arena Knockout (Sumo Lava):**
   * Platform melayang perlahan menyusut ke tengah.
   * Pukul dan dorong musuh keluar arena ke dalam lava panas!
2. **Tank Battle (Peluru Pantul):**
   * Pertempuran tank top-down di dalam labirin.
   * Peluru bisa memantul 3 kali di dinding. Satu tembakan langsung meledakkan tank lawan!
3. **Stickman Soccer (Sepak Bola 2v2):**
   * Pertandingan sepak bola mini 2v2 (Tim Merah vs Tim Biru).
   * Giring, sleding, dan tendang bola ke gawang lawan dalam 60 detik!
4. **Color The Floor (Cat Lantai):**
   * Lari mewarnai ubin lantai arena dengan warna stickman Anda.
   * Pemain dengan ubin terbanyak saat waktu habis dinobatkan sebagai pemenang!
5. **Bomb Tag (Oper Bom Waktu):**
   * Satu pemain memegang bom yang terus berdetik semakin cepat.
   * Tabrak lawan untuk mengoper bom tersebut sebelum meledak di tangan Anda!

---

## 🚀 Build Otomatis File `.apk` di GitHub Actions

Workflow GitHub Actions sudah siap di [`.github/workflows/unity-build.yml`](file:///root/stickman-party-unity/.github/workflows/unity-build.yml). Workflow ini secara otomatis mengompilasi file **Android `.apk`** yang bisa langsung di-install di HP Anda.

### Konfigurasi Rahasia (Secrets) di GitHub:
1. Masuk ke **Settings > Secrets and variables > Actions** di repositori GitHub Anda.
2. Tambahkan 3 variabel rahasia:
   * `UNITY_LICENSE`: Isi dengan teks lisensi Unity Anda (`.ulf` / XML).
   * `UNITY_EMAIL`: Email akun Unity ID Anda.
   * `UNITY_PASSWORD`: Kata sandi akun Unity ID Anda.
3. Setiap kali Anda melakukan `git push`, GitHub Actions akan otomatis membuat file `.apk` dan menyediakannya di tab **Actions > Artifacts** untuk langsung diunduh ke HP Android.
