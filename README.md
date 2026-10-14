# 🎮 Stickman Party Clone (Unity 2D Local Multiplayer)

Game pesta 1–4 pemain lokal bergaya **Stickman Party** yang dibangun dengan arsitektur Unity (C#) yang modular, ringan, dan siap di-build menggunakan **GitHub Actions**.

---

## 🌟 Fitur Utama

1. **1–4 Pemain Lokal (Single Screen / Single Keyboard)**
   - Mendukung hingga 4 pemain bermain bersamaan di 1 komputer/layar.
   - Dilengkapi **Bot AI Cerdas** yang otomatis mengisi slot kosong jika pemain kurang dari 4 (misal: 1 Pemain vs 3 Bot, atau 2 Pemain vs 2 Bot).
2. **Zero External Assets Dependency (Langsung Jalan)**
   - Visual karakter stickman dibuat secara prosedural dengan animasi kenyal (*bouncy squash & stretch*), mata ekspresif yang menoleh ke arah gerakan, dan kepalan tinju.
   - Efek suara arcade disintesis langsung lewat kode C# (*procedural audio*), sehingga tidak memerlukan unduhan file `.mp3` / `.wav` terpisah.
3. **Sistem Turnamen & Podium Juara**
   - Mainkan turnamen 5 ronde.
   - Pemenang setiap mini-game mendapatkan +3 Bintang.
   - Layar podium juara di akhir permainan lengkap dengan peringkat bintang para pemain.
4. **Auto-Bootstrap System**
   - Anda cukup membuka Unity dan menekan tombol **Play ▶** di scene mana saja; script `GameBootstrap.cs` otomatis mengatur Kamera, Canvas UI, EventSystem, dan Manajer game!

---

## 🕹️ Daftar 5 Mini-Game

| Mini-Game | Mode | Deskripsi & Mekanik |
|---|---|---|
| **1. Lava Arena Knockout** | Survival | Pemain berada di atas platform bundar yang perlahan menyusut. Gunakan tombol aksi untuk memukul dan mendorong musuh ke lava! |
| **2. Tank Battle** | Survival | Kendalikan tank di dalam labirin. Tembakkan peluru yang bisa memantul di dinding hingga 3 kali untuk menghancurkan tank lawan dan kotak kayu! |
| **3. Stickman Soccer** | Tim 2v2 | Sepak bola 2v2 (Tim Merah vs Tim Biru). Giring dan tendang bola ke gawang lawan dalam batas waktu 60 detik! |
| **4. Color The Floor** | Skor Area | Berlari di atas ubin lantai untuk mewarnainya dengan warna karaktermu. Pemain dengan ubin terbanyak saat waktu habis menang! |
| **5. Bomb Tag** | Hot Potato | Satu pemain memegang bom yang terus berdetik. Tabrak pemain lain untuk mengoper bom sebelum meledak di tanganmu! |

---

## ⌨️ Skema Kontrol (1 Keyboard Bersama)

| Pemain | Warna | Tombol Gerak | Tombol Aksi (Pukul / Tembak / Tendang) |
|---|---|---|---|
| **Player 1** | 🔴 Merah | `W` `A` `S` `D` | `Spasi` |
| **Player 2** | 🔵 Biru | `Panah Atas` `Bawah` `Kiri` `Kanan` | `Enter` |
| **Player 3** | 🟢 Hijau | `I` `J` `K` `L` | `U` |
| **Player 4** | 🟡 Kuning | `Numpad 8` `4` `5` `6` | `Numpad +` |

---

## 🚀 Cara Menjalankan di Unity Editor

1. Buka **Unity Hub**.
2. Klik tombol **Add** -> Pilih folder `stickman-party-unity`.
3. Buka proyek (Direkomendasikan versi **Unity 2021.3 LTS**, **Unity 2022.3 LTS**, atau **Unity 6**).
4. Tekan tombol **Play ▶** di editor. 
5. Pilih jumlah pemain (1P, 2P, 3P, atau 4P) pada menu utama dan nikmati permainannya!

---

## ⚙️ Build Otomatis di GitHub Actions

File alur kerja sudah tersedia di `.github/workflows/unity-build.yml`. Alur kerja ini menggunakan [GameCI](https://game.ci/) untuk secara otomatis mengompilasi:
* **StandaloneWindows64** (`.exe`)
* **WebGL** (Bisa dimainkan langsung di web)
* **StandaloneLinux64**

### Menyiapkan Secret Repository di GitHub:
Masuk ke **Settings > Secrets and variables > Actions** di repositori GitHub Anda dan tambahkan:
* `UNITY_LICENSE`: Isi dengan lisensi file Unity Anda (`.ulf`) atau XML lisensi.
* `UNITY_EMAIL`: Email akun Unity ID Anda.
* `UNITY_PASSWORD`: Kata sandi akun Unity ID Anda.

Setiap kali Anda melakukan `git push` ke branch `main`, GitHub Actions akan otomatis membuat build dan mengunggahnya sebagai artefak siap unduh!
