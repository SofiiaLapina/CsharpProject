## Лабораторна робота 3. Архітектура застосунків. MVVM, IoC

### Призначення
Ця лабораторна робота є продовженням лабораторної 2.  
У межах проєкту "Менеджер занять" виконано перебудову структури застосунку відповідно до багатошарової архітектури та патерну **MVVM**, а також впроваджено **Dependency Injection** через **IoC-контейнер**.
Додатковий функціонал не додавався; збережено логіку Лаби 2 (3 сторінки та навігація).

---

## Структура рішення (Solution)

### 1) `StudyManager.Storage` . DB Models
Містить моделі даних та enum:
- `SubjectData`, `LessonData`
- `KnowledgeArea`, `LessonType`

Особливості DB Models:
- не містять обчислюваних полів
- не зберігають об’єктні посилання на пов’язані сутності
- зв’язок між заняттям і предметом реалізовано через `SubjectId`

---

### 2) `StudyManager.Repositories` . Repositories + Storage
Рівень доступу до сховища даних. Повертає **DB Models**.
- `FakeStorage` —> штучне сховище з тестовими даними
- Інтерфейси репозиторіїв:
  - `ISubjectRepository`, `ILessonRepository`
- Реалізації репозиторіїв:
  - `SubjectRepository`, `LessonRepository`

Репозиторії використовуються через інтерфейси (принцип **Dependency Inversion**).

---

### 3) `StudyManager.Services` . Services + DTO Models
Рівень сервісів для підготовки даних під задачі UI. UI працює тільки через сервіси та DTO.

**DTO Models:**
- `Dtos/Subjects`:
  - `SubjectListItemDto` —> модель для списку предметів
  - `SubjectDetailsDto` —> модель для детальної сторінки предмета
- `Dtos/Lessons`:
  - `LessonListItemDto` —> модель для списку занять
  - `LessonDetailsDto` —> модель для детальної сторінки заняття

**Сервіси:**
- Інтерфейси (папка `Interfaces`):
  - `ISubjectService`, `ILessonService`
- Реалізації:
  - `SubjectService`, `LessonService`

Сервіси:
- отримують DB Models з репозиторіїв,
- конвертують їх у DTO,
- формують дані для відображення (зокрема тривалість занять та підсумкові значення для деталей предмета).

---

### 4) `StudyManager.WpfApp` . UI (WPF, MVVM)
UI застосунок реалізовано з використанням WPF та патерну **MVVM**.

**Infrastructure:**
- `INavigationService`, `NavigationService` —> навігація через `Frame` в одному головному вікні
- `RelayCommand` —> команди для взаємодії з UI

**ViewModels:**
- розміщені в папці `ViewModels` (MVVM-логіка сторінок і команд)

**Pages:**
- `SubjectsPage` —> список предметів
- `SubjectDetailsPage` —> деталі предмета + список занять
- `LessonDetailsPage` —> деталі заняття

**Важливо:** у `.xaml.cs` файлах відсутня логіка застосунку — лише `InitializeComponent()` та встановлення `DataContext` (BindingContext).

---

## Dependency Injection / IoC
Для забезпечення DI використано IoC-контейнер `Microsoft.Extensions.DependencyInjection`.
Реєстрація залежностей виконується у `StudyManager.WpfApp/App.xaml.cs`:
- репозиторії (`ISubjectRepository`, `ILessonRepository`)
- сервіси (`ISubjectService`, `ILessonService`)
- ViewModels та Pages
- `INavigationService` (працює через `MainWindow.MainFrame`)

UI-компоненти не створюють сервіси вручну; сервіси не створюють репозиторії вручну, усі залежності надаються контейнером.

---

## Логіка роботи застосунку
1. При запуску відображається сторінка зі списком предметів (`SubjectsPage`)
2. При виборі предмета відкривається сторінка деталей предмета (`SubjectDetailsPage`) зі списком занять
3. При виборі заняття відкривається сторінка деталей заняття (`LessonDetailsPage`)
4. Реалізовано навігацію назад для перегляду інших об’єктів

---

## Як запустити
З кореня репозиторію:

```bash
dotnet clean
dotnet build StudyManager.slnx
dotnet run --project StudyManager.WpfApp
