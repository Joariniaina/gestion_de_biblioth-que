Voici un **README complet en Markdown**, clair et professionnel, que tu pourras mettre directement dans ton dépôt GitHub pour ton projet *LibraryApp* (ASP.NET Core MVC + EF Core + Architecture en couches).

Tu peux bien sûr me demander de l’adapter selon ton projet exact.

---

# 📚 LibraryApp — Application de gestion de bibliothèque

**ASP.NET Core MVC | Entity Framework Core | Architecture en couches**

## 🌟 Description du projet

**LibraryApp** est une application web permettant de gérer une bibliothèque :

* Ajouter des livres
* Modifier des livres
* Lister les livres
* Ajouter des tags (mots-clés)
* Gestion du stock (TotalCopies)
* Architecture claire (séparation DTO, Business Logic, Data Access)

Ce projet est idéal pour apprendre ou démontrer :
✔ ASP.NET Core MVC
✔ Entity Framework Core
✔ Clean architecture / MVC en couches
✔ Razor Views
✔ CRUD complet
✔ Gestion des DTOs

---

## 🗂️ Architecture du projet

```plaintext
LibraryApp/
│
├── LibraryApp.Web/            -> Projet MVC (Controllers, Views, wwwroot)
│   ├── Controllers/
│   │   └── BooksController.cs
│   ├── Views/
│   │   └── Books/
│   │       ├── Index.cshtml
│   │       ├── Create.cshtml
│   │       ├── Edit.cshtml
│   │       └── Delete.cshtml
│   ├── DTOs/
│   │   └── BookDto.cs
│
├── LibraryApp.Core/           -> Logique métier / Services
│   ├── Interfaces/
│   ├── Services/
│
├── LibraryApp.Infrastructure/ -> EF Core + Accès aux données
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Entities/
│   │   └── Book.cs
│   ├── Repositories/
│
└── README.md
```

---

## 📘 Fonctionnalités principales

### 🔹 Ajouter un livre

* Titre
* Auteur
* Genre
* Année de publication
* Nombre total de copies disponibles
* Tags (mots-clés séparés par des virgules)

### 🔹 Modifier un livre

* Tous les champs sont éditables
* Les tags peuvent être changés facilement

### 🔹 Lister les livres

* Affichage clair sous forme de tableau

### 🔹 Supprimer un livre

* Suppression sécurisée avec confirmation

---

## 🧩 Exemple de modèle : BookDto

```csharp
public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int PublicationYear { get; set; }
    public int TotalCopies { get; set; }
    public string Tags { get; set; }
}
```

---

## 🖥️ Exemple de View : Create.cshtml

```html
<form asp-action="Create" method="post">
    <div class="mb-3">
        <label asp-for="Title" class="form-label"></label>
        <input asp-for="Title" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="Author" class="form-label"></label>
        <input asp-for="Author" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="Genre" class="form-label"></label>
        <input asp-for="Genre" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="PublicationYear" class="form-label"></label>
        <input asp-for="PublicationYear" type="number" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="TotalCopies" class="form-label"></label>
        <input asp-for="TotalCopies" type="number" class="form-control" />
    </div>

    <div class="mb-3">
        <label asp-for="Tags" class="form-label">Tags (séparés par des virgules)</label>
        <input asp-for="Tags" class="form-control" />
    </div>

    <button type="submit" class="btn btn-primary">Ajouter</button>
    <a asp-action="Index" class="btn btn-secondary">Annuler</a>
</form>
```

---

## 🛠️ Technologies utilisées

| Technologie               | Rôle                          |
| ------------------------- | ----------------------------- |
| **ASP.NET Core MVC**      | Framework principal           |
| **Entity Framework Core** | ORM pour la base de données   |
| **SQL Server / SQLite**   | Base de données               |
| **Bootstrap 5**           | Interface utilisateur         |
| **Razor Pages**           | Génération des vues           |
| **DTOs**                  | Séparation claire des données |

---

## 🚀 Installation et exécution

### 1️⃣ Cloner le projet

```bash
git clone https://github.com/votre-utilisateur/LibraryApp.git
cd LibraryApp
```

### 2️⃣ Installer les dépendances

```bash
dotnet restore
```

### 3️⃣ Exécuter les migrations

```bash
dotnet ef database update
```

### 4️⃣ Lancer l’application

```bash
dotnet run --project LibraryApp.Web
```

👉 L’application sera disponible sur :
**[http://localhost:5000](http://localhost:5000)**

---

## 🧪 Améliorations possibles

* Authentification (Admin / User)
* Gestion des emprunts et retours
* API RESTful
* Search + filtres
* Tags sous forme de liste dynamique
* Upload d’image pour les livres
* Dashboard avec statistiques

---

## 🤝 Contributions

Les contributions sont les bienvenues !
N'hésitez pas à ouvrir une **issue** ou une **pull request**.

---

## 📄 Licence

Ce projet est sous licence MIT.
Vous pouvez l’utiliser librement.

---
