# 🎮 Loadout

**Loadout** je web aplikacija za prodaju digitalnih igara i dodatnog sadržaja (DLC, dodaci, itemi), inspirisana platformama poput Steama. Korisnici mogu pregledati katalog, kupovati i rezervisati (pre-order) igre, preuzimati licencne ključeve i pratiti svoje kupovine, a administratori upravljaju igrama, korisnicima, narudžbama, vijestima i sadržajem.

Seminarski rad iz predmeta **Razvoj softvera I** (2025/26).

| Član tima | Indeks |
|---|---|
| Rijad Delalić | IB220051 |
| Velid Madžak | IB220203 |

---

## Tehnologije

**Backend:** ASP.NET Core 8 Web API · Entity Framework Core 8 (SQL Server) · JWT autentifikacija · MediatR · Stripe.net · BackgroundService (scheduler)

**Frontend:** Angular 21 · Angular Material · Bootstrap 5 · ngx-translate (BS/EN) · Stripe.js · jsPDF · qrcode

---

## Glavne funkcionalnosti

### Kupac
- **Katalog igara** – responzivni grid, pretraga u realnom vremenu, filteri (žanr, cijena, ocjena, tip sadržaja, datum izlaska), skeleton učitavanje
- **Detalji igre** – opis, sistemski zahtjevi, trailer, recenzije, dijeljenje putem QR koda
- **Korpa** – dodavanje, uklanjanje, *„sačuvaj za kasnije“*
- **Checkout wizard** (4 koraka) – korpa → podaci kupca → plaćanje → potvrda
- **Stripe plaćanje** – kartično plaćanje (test mode), uključujući 3D Secure; transakcije se spremaju u bazu
- **Pre-order** – rezervacija igara koje nisu izašle, otkazivanje prije izlaska, automatsko izdavanje ključeva na dan izlaska
- **Moje kupovine** – historija narudžbi, licencni ključevi (kopiranje, QR kod), PDF potvrda o kupovini
- **Vijesti i obavještenja**, registracija/prijava, lista želja, recenzije, višejezičnost (BS/EN)

### Administrator
- Dashboard sa statistikom prodaje
- Upravljanje igrama, kategorijama, promocijama i narudžbama
- Upravljanje korisnicima – CRUD, filteri, sortiranje, paging
- Upravljanje vijestima – CRUD, zakazivanje objave i isteka, pregled uživo
- Pozadinski procesi – pregled stanja, historija izvršavanja, ručno pokretanje

---

## Struktura projekta

```
Loadout/
├── Market.Backend/
│   ├── Market.API              # kontroleri, middleware, Program.cs
│   ├── Market.Application      # interfejsi servisa, zajednička logika (paging, izuzeci)
│   ├── Market.Domain           # entiteti (modeli baze)
│   ├── Market.Infrastructure   # DbContext, migracije, servisi, pozadinski procesi
│   ├── Market.Shared           # DTO-ovi, enumeracije, opcije
│   ├── Market.Tests            # unit i integracioni testovi
│   └── Scripts/                # SQL skripta s testnim podacima
└── Market.Frontend/
    └── rs1-frontend-2025-26/   # Angular aplikacija
        ├── public/i18n/        # prijevodi (bs.json, en.json)
        └── src/app/
            ├── api-services/   # servisi za komunikaciju s API-jem
            ├── core/           # auth, guardovi, interceptori, zajednički servisi
            └── modules/        # public, client, admin, auth, shared
```

---

## Pokretanje

### Preduslovi
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) 20+ i npm
- SQL Server (Express ili Developer)
- Visual Studio 2022 (ili Rider) i WebStorm/VS Code

### 1. Baza podataka
1. U `Market.Backend/Market.API/appsettings.json` podesiti connection string (`ConnectionStrings:Main`) prema svom SQL Serveru.
2. U Visual Studiju otvoriti `Market.Backend/Market.Backend.sln`, pa u **Package Manager Console** (Default project: `Market.Infrastructure`):
   ```
   Update-Database
   ```
3. *(Opcionalno)* Pokrenuti skriptu `Market.Backend/Scripts/seed-loadout-data.sql` (npr. u SSMS-u nad bazom `LoadoutDB`) – dodaje 47 igara, kategorije, ocjene i vijesti. Skripta je idempotentna i može se pokrenuti više puta.

### 2. Stripe ključevi (za plaćanje)
Ključevi se **ne nalaze u repozitoriju**. U Visual Studiju: desni klik na `Market.API` → **Manage User Secrets** i unijeti testne ključeve sa [Stripe Dashboarda](https://dashboard.stripe.com/test/apikeys):
```json
{
  "Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_..."
  }
}
```
Bez ključeva aplikacija radi normalno, samo je kartično plaćanje onemogućeno.

### 3. Backend
Pokrenuti projekat `Market.API` s profilom **https** → API je dostupan na `https://localhost:7260`, a Swagger na `https://localhost:7260/swagger`.

### 4. Frontend
```bash
cd Market.Frontend/rs1-frontend-2025-26
npm install
npm start
```
Aplikacija je dostupna na `http://localhost:4200`.

---

## Testiranje plaćanja (Stripe test mode)

| Kartica | Rezultat |
|---|---|
| `4242 4242 4242 4242` | uspješno plaćanje |
| `4000 0025 0000 3155` | zahtijeva 3D Secure potvrdu |
| `4000 0000 0000 0002` | odbijena kartica |

Datum isteka: bilo koji u budućnosti (npr. `12/30`), CVC: bilo koje 3 cifre. U test modu se ne naplaćuje pravi novac.
