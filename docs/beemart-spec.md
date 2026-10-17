# BeeMart — Project Specification

> .NET C# IT BOOTCAMP TEST 2026/2027 — Binus Point of Sale (POS) Web App
> Full reference spec transcribed from `BeeMart_ITBootcamp_Basic_2027.pdf` (text layer + image/mockup/ERD content). Loaded on demand — not a steering file.

## Overview

BeeMart adalah proyek aplikasi dari IT Division untuk membangun platform Sistem Kasir terintegrasi menggunakan C# pada framework .NET 8. Arsitektur **Separation of Concerns**: sistem dipisah menjadi dua proyek utama — **WebClient (Frontend/UI)** dan **Backend API**.

WebClient hanya bertindak sebagai consumer data. Seluruh proses bisnis, logika aplikasi, dan persistensi data dikelola terpusat di Backend API.

### Alur Bisnis Inti (POS)

- **Register & Login** dengan pembagian hak akses (Role): Admin atau Kasir.
- **New Transaction** dengan jam real-time, pencarian barang, input quantity, pemilihan metode pembayaran (Cash, Debit, Credit), dan kalkulasi subtotal serta total harga.
- **Invoice Number otomatis** per transaksi dengan format baku `[5 digit userID][ddmmyy][3 digit running number]` — tidak dapat diedit user.
- **History Transaction** berdasarkan filter tanggal, diurutkan Ascending berdasarkan Tanggal & Jam transaksi.
- **Akses sesuai peran**: Kasir hanya melihat transaksinya sendiri; Admin melihat data seluruh Kasir.

### Tujuan Pembelajaran

1. **Arsitektur Sistem**: Separation of Concerns (pemisahan UI dan API).
2. **Integrasi API**: Desain & konsumsi RESTful API untuk autentikasi, master produk, pembuatan transaksi, pelaporan riwayat.
3. **State Management**: Kelola state di WebClient (data kasir login, cart, state pencarian barang).
4. **Keamanan & Validasi**: Validasi input di klien & server, serta Role-based Authorization.
5. **Desain Database**: Merancang relasi data (ERD) antar tabel User, Product, Transaction, TransactionDetail.

## Aturan Teknis

- **Platform**: Wajib ASP.NET Core C# dengan Framework .NET 8.
- **Frontend (WebClient)**: HTML/JS Murni, ASP.NET Core Razor Pages, atau MVC. UI wajib sesuai mockup requirement. **Tidak boleh pakai framework CSS Bootstrap.** Frontend **dilarang keras** akses langsung ke database — semua data lewat HTTP Request ke Backend API.
- **Backend API**: ASP.NET Core API dengan .NET 8 (Controller-based atau Minimal API). EF Core terhubung ke SQL Server sebagai ORM.
- **Standar API & Pagination**: Format JSON dan terapkan endpoint versioning. Endpoint Riwayat Transaksi wajib Pagination (maksimal 20 data per halaman).
- **Error Handling**: Penanganan error informatif via UI Popup (Toast / Alert) atau Label Message.
- **I18N/Format**: Format lokal Indonesia untuk mata uang Rupiah (IDR) di seluruh UI; waktu disesuaikan dengan zona waktu lokal.

## Kriteria Penerimaan (Acceptance Criteria)

- Seluruh halaman (Login, Register, New Transaction, History Transaction) tersedia dan fungsional sesuai spesifikasi.
- WebClient 100% bergantung pada API (tidak ada koneksi langsung ke database dari WebClient).
- Autentikasi & otorisasi berjalan baik (endpoint riwayat transaksi memisahkan hak akses Admin vs Kasir secara akurat).
- Nomor Invoice ter-generate otomatis dengan format tepat dan berurutan.

## Instruksi Pengumpulan Proyek

Mahasiswa WAJIB mengumpulkan proyek lengkap (termasuk folder aset gambar, `appsettings.json`, `.sln`, dan `.csproj`) dalam arsip (ZIP/RAR):

- Folder Frontend (UI/WebClient)
- Folder Backend (API)
- **Query SQL (Script Database)** — sertakan script `.sql` berisi Query Create Table (DDL) dan Query Insert Data (DML/Seeding produk dan user) agar aplikasi bisa langsung diuji coba.

> **Peringatan Penting:** Segala bentuk kecurangan, plagiarisme, atau pelanggaran aturan teknis (seperti akses database langsung dari WebClient) akan mengakibatkan diskualifikasi dan tidak mendapatkan nilai.

## ERD Database

Empat tabel utama plus satu lookup table. Format kolom: `DataType | ColumnName | Key`.

### MsUser
| DataType | Column | Key |
|---|---|---|
| NVARCHAR(5) | IdUser | PK |
| NVARCHAR(50) | Username | |
| NVARCHAR(300) | FullName | |
| NVARCHAR(255) | PasswordHash | |
| NVARCHAR(20) | Role | |
| BIT | Stsrc | |
| DATETIME2 | Datein | |
| NVARCHAR(36) | Userin | |
| DATETIME2 | Dateup | |
| NVARCHAR(36) | Userup | |

### LtPaymentMethod
| DataType | Column | Key |
|---|---|---|
| INT | IdPaymentMethod | PK |
| NVARCHAR(50) | MethodName | |
| BIT | Stsrc | |
| DATETIME2 | Datein | |
| NVARCHAR(36) | Userin | |
| DATETIME2 | Dateup | |
| NVARCHAR(36) | Userup | |

### TrTransaction
| DataType | Column | Key |
|---|---|---|
| NVARCHAR(36) | IdTransaction | PK |
| NVARCHAR(20) | InvoiceNo | |
| DATETIME2 | TransactionDate | |
| NVARCHAR(5) | IdUser | FK → MsUser.IdUser |
| INT | IdPaymentMethod | FK → LtPaymentMethod.IdPaymentMethod |
| DECIMAL(18,2) | TotalPrice | |
| BIT | Stsrc | |
| DATETIME2 | Datein | |
| NVARCHAR(36) | Userin | |
| DATETIME2 | Dateup | |
| NVARCHAR(36) | Userup | |

### MsProduct
| DataType | Column | Key |
|---|---|---|
| NVARCHAR(36) | IdProduct | PK |
| NVARCHAR(100) | Name | |
| DECIMAL(10,2) | Weight | |
| INT | Stock | |
| DECIMAL(18,2) | Price | |
| DECIMAL(5,2) | Discount | |
| BIT | Stsrc | |
| DATETIME2 | Datein | |
| NVARCHAR(36) | Userin | |
| DATETIME2 | Dateup | |
| NVARCHAR(36) | Userup | |

### TrTransactionDetail
| DataType | Column | Key |
|---|---|---|
| NVARCHAR(36) | IdTransactionDetail | PK |
| NVARCHAR(36) | IdTransaction | FK → TrTransaction.IdTransaction |
| NVARCHAR(36) | IdProduct | FK → MsProduct.IdProduct |
| INT | Qty | |
| DECIMAL(18,2) | Price | |
| DECIMAL(5,2) | Discount | |
| DECIMAL(18,2) | SubTotal | |
| BIT | Stsrc | |
| DATETIME2 | Datein | |
| NVARCHAR(36) | Userin | |
| DATETIME2 | Dateup | |
| NVARCHAR(36) | Userup | |

### Relationships
- MsUser (1) — (many) TrTransaction — one user has many transactions
- LtPaymentMethod (1) — (many) TrTransaction — one payment method used by many transactions
- TrTransaction (1) — (many) TrTransactionDetail — one transaction has many detail lines
- MsProduct (1) — (many) TrTransactionDetail — one product appears in many detail lines

> Note: ERD transcribed from the diagram image. Table/column names and PK/FK relationships are faithful, but verify `MsProduct`/`TrTransactionDetail` columns against the source PDF before generating DDL directly.

## Fitur yang Harus Diimplementasikan

### 1. Login

Gerbang utama untuk autentikasi & otorisasi pengguna.

**Validasi:**
- a. Username dan Password wajib diisi.
- b. Kombinasi Username, Password, dan Role harus divalidasi dengan data pada tabel `MsUser`.
- c. Pencocokan password harus menggunakan metode hashing. Password di database tidak boleh plain text.
- d. Jika username, password, atau role tidak sesuai, kembalikan pesan error `"Invalid login"`.
- e. Dropdown Role harus memiliki pilihan:
  - i. Name: `"Admin"`, Value: `"Admin"`
  - ii. Name: `"Kasir"`, Value: `"Cashier"`

**Mockup — Login page:** Card berjudul "BeeMart Login" berisi: input "Username", input "Password", dropdown "Role" (default "Kasir"), tombol biru "Login", dan link "Belum punya akun? Register".

### 2. Register

**Mockup — Register page:** Card berjudul "Register Akun" berisi: input "Username", "Full Name", "Password", "Re-Password", dropdown "Role" (default "Kasir"), tombol biru "Daftar", dan link "Sudah punya akun? Login".

**Validasi:**
- a. Username, Full Name, Password & Re-Password harus diisi.
- b. Password yang disimpan ke database harus di-hashing terlebih dahulu.
- c. Username yang didaftarkan tidak boleh duplikat; jika sudah ada, kembalikan pesan error `"username has already exists"`.

### 3. New Transaction

Bisa diakses role Cashier dan Admin; digunakan untuk transaksi pembelian barang.

> Catatan: Poin a–c pada PDF asli adalah teks sisa dari seksi Register (duplikat) — validasi relevan untuk New Transaction dimulai dari poin d. Dipertahankan apa adanya di bawah untuk kesetiaan pada sumber.

**Validasi:**
- a. Username, Full Name, Password, dan Re-Password wajib diisi. *(carryover teks Register)*
- b. Password yang disimpan ke database harus di-hash terlebih dahulu. *(carryover teks Register)*
- c. Username tidak boleh duplikat (harus unik). Jika sudah terdaftar, kembalikan pesan error `"Username already exists"`. *(carryover teks Register)*
- d. Fitur pencarian barang harus menampilkan **maksimal 10 data** dari database yang memiliki nama serupa dengan kata kunci.
- e. Pada tabel hasil pencarian, pengguna dapat memilih barang dengan tombol **"Pilih"**.
- f. Ketika "Pilih" diklik, sistem menampilkan kotak dialog berisi info barang, harga satuan, dan sisa stok. Pengguna input qty lalu klik **"Masukkan ke Keranjang"**. Jika qty melebihi sisa stok, tampilkan error: `"Jumlah barang yang dimasukkan melebihi sisa stok."`
- g. Pada daftar belanjaan, pengguna dapat menghapus item yang telah ditambahkan.
- h. Dropdown Metode Pembayaran harus di-fetch secara dinamis via API.
- i. Jika submit transaksi berhasil, sistem reset halaman ke kondisi awal dan menghasilkan Invoice Number baru untuk transaksi berikutnya.

**Mockup — New Transaction (initial/empty state):**
- Header bar: "BeeMart" (kiri); "4/9/2026, 10.32.46 | Kasir: steven" dan tombol merah "Logout" (kanan).
- Dua tab: "New Transaction" (aktif), "History Transaction".
- Field read-only "Invoice No: 76985040926001".
- Search label "Cari Barang (Ketik: Apel / Susu, lalu tekan Enter)", input placeholder "Ketik nama barang...".
- Section "Daftar Belanjaan", tabel kosong. Kolom: Nama Barang | Harga | Diskon | Qty | Subtotal | Aksi.
- "Total: Rp 0". Dropdown "Metode Pembayaran:" (Cash). Tombol "Submit Transaksi".

**Mockup — search results + cart:**
- Search input: "Apel". Tabel hasil cari — kolom: Nama Barang | Berat | Stock | Harga | Diskon | Aksi.
  - Apel Fuji | 1 Kg | 50 | Rp 35.000 | 0% | [Pilih]
  - Apel Merah | 1 Kg | 30 | Rp 30.000 | 5% | [Pilih]
  - Apel Hijau | 1 Kg | 25 | Rp 28.000 | 0% | [Pilih]
- Daftar Belanjaan:
  - Apel Fuji | Rp 35.000 | 0% | 1 | Rp 35.000 | [Hapus]
  - Susu UHT Full Cream | Rp 18.000 | 0% | 3 | Rp 54.000 | [Hapus]
- Total: Rp 89.000.

**Mockup — item selected dialog:**
- Panel "Barang Terpilih: Apel Merah" — Harga Satuan: Rp 30.000, Sisa Stock: 30, Qty: 10, tombol "Masukkan ke Keranjang".
- Daftar Belanjaan: Apel Fuji (1, Rp 35.000) + Susu UHT Full Cream (1, Rp 18.000). Total: Rp 53.000.

**Mockup — filled cart before submit:**
- Header time "10.34.50".
- Daftar Belanjaan:
  - Apel Fuji | Rp 35.000 | 0% | 1 | Rp 35.000
  - Susu UHT Full Cream | Rp 18.000 | 0% | 3 | Rp 54.000
  - Apel Merah | Rp 30.000 | 5% | 10 | Rp 285.000
- Total: Rp 374.000.

**Mockup — after successful submit (reset state):**
- Header time "10.35.31". Invoice No: **76985040926002** (running number naik dari ...001 ke ...002).
- Daftar Belanjaan kosong. Total: Rp 0.

### 4. Riwayat Transaksi (Role: Kasir)

Menampilkan data transaksi berdasarkan tanggal yang dipilih.

**Ketentuan & validasi:**
- a. Untuk pencarian data, pengguna wajib memilih tanggal transaksi terlebih dahulu.
- b. Input tanggal wajib menggunakan komponen **date picker**.
- c. Tabel transaksi harus pagination (maksimal 20 data per page). Diurutkan (order by) tanggal transaksi **ascending**.
- d. Header halaman menampilkan sesi pengguna dengan format: `Role: <role user> | Login: <username user>`.

**Mockup — History (Kasir):**
- Header kanan: "Role: Cashier | Login: steven" + tombol merah "Logout".
- Subheading "Pencarian Data Transaksi (Hanya Kasir: steven)".
- "Pilih Tanggal:" date picker 09/04/2026, tombol biru "Cari Data".
- Tabel — kolom: No. Transaksi | Tanggal & Jam | Kasir | Jml Item | Pembayaran | Total Harga.
  - 76985040926001 | 2026-09-04 10:35 | Steven | 14 | Cash | Rp 374.000
- Pagination: page "1".

### 5. Riwayat Transaksi (Role: Admin)

Menampilkan data transaksi berdasarkan tanggal yang dipilih.

**Ketentuan & validasi:**
- a. Untuk pencarian data, pengguna wajib memilih tanggal transaksi terlebih dahulu.
- b. Input tanggal wajib menggunakan komponen **date picker**.
- c. Data yang dimunculkan mencakup **seluruh** riwayat transaksi dari semua kasir.
- d. Tabel transaksi harus pagination (maksimal 20 data per page). Diurutkan (order by) tanggal transaksi **ascending**.
- e. Header halaman menampilkan sesi pengguna dengan format: `Role: <role user> | Login: <username user>`.

---

*GOOD LUCK*
