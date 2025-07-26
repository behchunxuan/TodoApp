# 📝 TodoApp Fullstack

A fullstack Todo List management system built with modern technologies:

- ⚙️ **Backend:** ASP.NET Core 8 Web API using Clean Architecture  
- 🌐 **Frontend:** React + Tailwind CSS  
- 🗄️ **Database:** SQLite

---

## 🚀 Features

- ✅ Add, edit, complete, and cancel todos  
- 🔍 Global filtering & sorting  
- 🗂️ View by status: Pending, Completed, or Cancelled  
- 🧠 Smart status flow logic  
- 🛑 Modal confirmations using SweetAlert2  
- 🧪 Unit testing with xUnit and Moq for the API layer  
- 🧱 Clean, modular architecture for better scalability and maintainability

---

## 🏗️ Project Structure

```
TodoApp/
├── TodoApp.API/            # ASP.NET Core Web API (Backend entry point)
├── todo-frontend/          # React + Tailwind frontend
├── TodoApp.Application/    # Application layer (DTOs, Services, Validators)
├── TodoApp.Domain/         # Domain entities and enums
├── TodoApp.Infrastructure/ # DB context, Repositories, SeedData
└── README.md               # Project documentation
```

---

## ⚙️ Backend Setup

1. Open the solution `TodoApp.API.sln` in **Visual Studio** or your preferred IDE.  
2. Make sure the `TodoApp.API` project is set as the startup project.  
3. Run it using **F5** or via terminal:

   ```bash
   cd TodoApp.API
   dotnet run
   ```

4. Open Swagger UI in browser:  
   ➤ `https://localhost:{PORT}/swagger`

### 🗄️ Database Info

- The app uses **SQLite** by default.
- On first run, a `todo.db` file will be created automatically.
- Sample Seed Data will be inserted via `SeedData.cs`.

> You can customize the connection string in `appsettings.json`.

---

## 💻 Frontend Setup

1. Open terminal and go to the frontend directory:

   ```bash
   cd todo-frontend
   ```

2. Install packages:

   ```bash
   npm install
   ```

3. Start the dev server:

   ```bash
   npm start
   ```

---

## 🧪 Running Unit Tests (Backend)

To run unit tests for the API logic:

```bash
dotnet test
```

- Make sure the test project has correct references to other layers.
- Tests are built using `xUnit` and `Moq`.

---

## 📌 Notes

- Make sure both **API** and **Frontend** are running to test full functionality.  
- Frontend uses **React** + **Tailwind** for fast modern development.