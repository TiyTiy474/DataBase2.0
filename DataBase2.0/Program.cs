using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataBase2._0
{
    class Program
    {
        // Static context for the entire application. Disposed on exit.
        private static MusicStoreContext _context;

        static void Main(string[] args)
        {
            _context = new MusicStoreContext();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление музыкальным магазином ===");
                Console.WriteLine("1. Управление дисками");
                Console.WriteLine("2. Управление исполнителями");
                Console.WriteLine("3. Управление продажами");
                Console.WriteLine("4. Управление скидочными картами");
                Console.WriteLine("5. Управление сотрудниками");
                Console.WriteLine("6. Просмотр отчётов");
                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите опцию: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        ManageDiscs();
                        break;
                    case "2":
                        ManageArtists();
                        break;
                    case "3":
                        ManageSales();
                        break;
                    case "4":
                        ManageDiscountCards();
                        break;
                    case "5":
                        ManageEmployers();
                        break;
                    case "6":
                        ViewReports();
                        break;
                    case "0":
                        // Dispose the context to free resources before exit.
                        _context.Dispose();
                        return;
                    default:
                        Console.WriteLine("Неверная опция!");
                        break;
                }
            }
        }

        private static void ManageDiscs()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление дисками ===");
                Console.WriteLine("1. Добавить новый диск");
                Console.WriteLine("2. Просмотреть все диски");
                Console.WriteLine("3. Обновить информацию о диске");
                Console.WriteLine("4. Удалить диск");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("\nВыберите опцию: ");

                string opt = Console.ReadLine() ?? "";
                switch (opt)
                {
                    case "1":
                        AddDisc();
                        break;
                    case "2":
                        ViewDiscs();
                        break;
                    case "3":
                        UpdateDisc();
                        break;
                    case "4":
                        DeleteDisc();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void AddDisc()
        {
            try
            {
                Console.WriteLine("\nВведите данные диска:");
                Console.Write("Название: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("ID исполнителя: ");
                if (!int.TryParse(Console.ReadLine(), out int artistId))
                {
                    Console.WriteLine("Некорректный ID исполнителя!");
                    return;
                }

                Console.Write("ID жанра: ");
                if (!int.TryParse(Console.ReadLine(), out int genreId))
                {
                    Console.WriteLine("Некорректный ID жанра!");
                    return;
                }

                Console.Write("Цена: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    Console.WriteLine("Некорректная цена!");
                    return;
                }

                Console.Write("Караоке (true/false): ");
                if (!bool.TryParse(Console.ReadLine(), out bool isKaraoke))
                {
                    Console.WriteLine("Некорректное значение для караоке!");
                    return;
                }

                var disc = new Disc
                {
                    DiscName = name,
                    ArtistId = artistId,
                    GenreId = genreId,
                    Price = price,
                    IsKaraoke = isKaraoke
                };

                _context.Discs.Add(disc);
                _context.SaveChanges();
                Console.WriteLine("Диск успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении диска: {ex.Message}");
            }
        }

        private static void ViewDiscs()
        {
            var discs = _context.Discs.Include(d => d.Artist).ToList();
            Console.WriteLine("\nВсе диски:");
            foreach (var disc in discs)
            {
                Console.WriteLine($"ID: {disc.ID}, Название: {disc.DiscName}, " +
                                  $"Исполнитель: {disc.Artist?.Name}, Цена: {disc.Price:C}");
            }
        }

        private static void ManageSales()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление продажами ===");
                Console.WriteLine("1. Зарегистрировать новую продажу");
                Console.WriteLine("2. Просмотреть все продажи");
                Console.WriteLine("3. Просмотреть продажи по дате");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("\nВыберите опцию: ");

                string opt = Console.ReadLine() ?? "";
                switch (opt)
                {
                    case "1":
                        RegisterSale();
                        break;
                    case "2":
                        ViewSales();
                        break;
                    case "3":
                        ViewSalesByDate();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void RegisterSale()
        {
            try
            {
                Console.WriteLine("\nВведите данные продажи:");

                Console.Write("ID диска: ");
                if (!int.TryParse(Console.ReadLine(), out int discId))
                {
                    Console.WriteLine("Некорректный ID диска!");
                    return;
                }

                var disc = _context.Discs.Find(discId);
                if (disc == null)
                {
                    Console.WriteLine("Диск не найден!");
                    return;
                }

                Console.Write("ID продавца: ");
                if (!int.TryParse(Console.ReadLine(), out int sellerId))
                {
                    Console.WriteLine("Некорректный ID продавца!");
                    return;
                }

                var seller = _context.Employers.Find(sellerId);
                if (seller == null)
                {
                    Console.WriteLine("Продавец не найден!");
                    return;
                }

                Console.Write("Место продажи: ");
                string location = Console.ReadLine() ?? "Неизвестно";

                decimal salePrice = disc.Price;
                int? discountCardId = null;

                Console.Write("Использовать скидочную карту? (да/нет): ");
                if ((Console.ReadLine()?.ToLower() ?? "нет") == "да")
                {
                    Console.Write("ID скидочной карты: ");
                    if (int.TryParse(Console.ReadLine(), out int cardId))
                    {
                        var discountCard = _context.DiscountCards.Find(cardId);
                        if (discountCard != null)
                        {
                            discountCardId = cardId;
                            salePrice = salePrice * (1 - discountCard.DiscountPercentage / 100m);
                        }
                        else
                        {
                            Console.WriteLine("Скидочная карта не найдена! Продажа будет оформлена без скидки.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ID скидочной карты! Продажа будет оформлена без скидки.");
                    }
                }

                var sale = new Sale
                {
                    DiscId = discId,
                    SellerID = sellerId,
                    Location = location,
                    SaleDate = DateTime.Now,
                    SalePrice = salePrice,
                    DiscountCardID = discountCardId
                };

                _context.Sales.Add(sale);
                _context.SaveChanges();

                Console.WriteLine("\nПродажа успешно зарегистрирована!");
                Console.WriteLine($"Итоговая цена: {salePrice:C}");
                if (discountCardId.HasValue)
                {
                    var discount = _context.DiscountCards.Find(discountCardId.Value);
                    Console.WriteLine($"Применена скидка: {discount?.DiscountPercentage}%");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при регистрации продажи: {ex.Message}");
            }
        }

        private static void SalesByPeriod()
        {
            try
            {
                Console.Write("\nВведите начальную дату (дд.мм.гггг): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    Console.WriteLine("Некорректная дата!");
                    return;
                }

                Console.Write("Введите конечную дату (дд.мм.гггг): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    Console.WriteLine("Некорректная дата!");
                    return;
                }

                var sales = _context.Sales
                    .Include(s => s.SoldDisc)
                    .Include(s => s.Seller)
                    .Where(s => s.SaleDate.Date >= startDate.Date && s.SaleDate.Date <= endDate.Date)
                    .ToList();

                Console.WriteLine($"\nОтчет по продажам за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
                Console.WriteLine(new string('-', 80));

                if (!sales.Any())
                {
                    Console.WriteLine("За указанный период продажи отсутствуют.");
                    return;
                }

                decimal totalRevenue = sales.Sum(s => s.SalePrice);
                var dailySales = sales.GroupBy(s => s.SaleDate.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count(), Revenue = g.Sum(s => s.SalePrice) });

                foreach (var day in dailySales.OrderBy(d => d.Date))
                {
                    Console.WriteLine($"{day.Date:dd.MM.yyyy}: {day.Count} продаж на сумму {day.Revenue:C}");
                }

                Console.WriteLine(new string('-', 80));
                Console.WriteLine($"Всего продаж: {sales.Count}");
                Console.WriteLine($"Общая выручка: {totalRevenue:C}");
                Console.WriteLine($"Средний чек: {(sales.Count > 0 ? totalRevenue / sales.Count : 0):C}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании отчета: {ex.Message}");
            }
        }

        private static void TopSellingArtists()
        {
            try
            {
                var topArtists = _context.Sales
                    .Include(s => s.SoldDisc)
                    .ThenInclude(d => d.Artist)
                    .GroupBy(s => s.SoldDisc.Artist)
                    .Select(g => new
                    {
                        Artist = g.Key,
                        SalesCount = g.Count(),
                        Revenue = g.Sum(s => s.SalePrice),
                        UniqueDiscs = g.Select(s => s.SoldDisc).Distinct().Count()
                    })
                    .OrderByDescending(x => x.Revenue)
                    .Take(10)
                    .ToList();

                Console.WriteLine("\nТоп 10 исполнителей по продажам:");
                Console.WriteLine(new string('-', 80));

                int rank = 1;
                foreach (var item in topArtists)
                {
                    Console.WriteLine($"{rank}. {item.Artist?.Name ?? "Неизвестно"}");
                    Console.WriteLine($"   Продано дисков: {item.SalesCount} шт.");
                    Console.WriteLine($"   Уникальных альбомов: {item.UniqueDiscs}");
                    Console.WriteLine($"   Общая выручка: {item.Revenue:C}");
                    rank++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании отчета: {ex.Message}");
            }
        }

        private static void BestPerformingEmployees()
        {
            var topSellers = _context.Sales
                .Include(s => s.Seller)
                .GroupBy(s => s.Seller)
                .Select(g => new
                {
                    Seller = g.Key,
                    SalesCount = g.Count(),
                    TotalRevenue = g.Sum(s => s.SalePrice)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(5)
                .ToList();

            Console.WriteLine("\nТоп-5 продавцов:");
            foreach (var seller in topSellers)
            {
                Console.WriteLine($"Продавец: {seller.Seller.Name}, " +
                                  $"Количество продаж: {seller.SalesCount}, " +
                                  $"Общая выручка: {seller.TotalRevenue:C}");
            }
        }

        private static void ManageArtists()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление исполнителями ===");
                Console.WriteLine("1. Добавить исполнителя");
                Console.WriteLine("2. Просмотреть всех исполнителей");
                Console.WriteLine("3. Обновить информацию об исполнителе");
                Console.WriteLine("4. Удалить исполнителя");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("\nВыберите опцию: ");

                string opt = Console.ReadLine() ?? "";
                switch (opt)
                {
                    case "1":
                        AddArtist();
                        break;
                    case "2":
                        ViewArtists();
                        break;
                    case "3":
                        UpdateArtist();
                        break;
                    case "4":
                        DeleteArtist();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void AddArtist()
        {
            try
            {
                Console.WriteLine("\nВведите данные исполнителя:");
                Console.Write("Имя: ");
                string name = Console.ReadLine() ?? "";

                var artist = new Artist { Name = name };
                _context.Artists.Add(artist);
                _context.SaveChanges();
                Console.WriteLine("Исполнитель успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении исполнителя: {ex.Message}");
            }
        }

        private static void ViewArtists()
        {
            try
            {
                var artists = _context.Artists
                    .Include(a => a.Discs)
                    .ToList();

                Console.WriteLine("\nСписок всех исполнителей:");
                foreach (var artist in artists)
                {
                    Console.WriteLine($"ID: {artist.ID}, Имя: {artist.Name}, " +
                                      $"Количество дисков: {artist.Discs.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка исполнителей: {ex.Message}");
            }
        }

        private static void UpdateArtist()
        {
            try
            {
                Console.Write("\nВведите ID исполнителя для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var artist = _context.Artists.Find(id);
                if (artist == null)
                {
                    Console.WriteLine("Исполнитель не найден!");
                    return;
                }

                Console.Write("Введите новое имя исполнителя: ");
                string newName = Console.ReadLine() ?? "";

                artist.Name = newName;
                _context.SaveChanges();
                Console.WriteLine("Информация об исполнителе успешно обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении информации об исполнителе: {ex.Message}");
            }
        }

        private static void DeleteArtist()
        {
            try
            {
                Console.Write("\nВведите ID исполнителя для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var artist = _context.Artists.Find(id);
                if (artist == null)
                {
                    Console.WriteLine("Исполнитель не найден!");
                    return;
                }

                var hasDiscs = _context.Discs.Any(d => d.ArtistId == id);
                if (hasDiscs)
                {
                    Console.WriteLine("Невозможно удалить исполнителя, так как с ним связаны диски!");
                    return;
                }

                _context.Artists.Remove(artist);
                _context.SaveChanges();
                Console.WriteLine("Исполнитель успешно удален!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении исполнителя: {ex.Message}");
            }
        }

        private static void ManageDiscountCards()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление скидочными картами ===");
                Console.WriteLine("1. Добавить новую карту");
                Console.WriteLine("2. Просмотреть все карты");
                Console.WriteLine("3. Обновить информацию о карте");
                Console.WriteLine("4. Удалить карту");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("\nВыберите опцию: ");

                string opt = Console.ReadLine() ?? "";
                switch (opt)
                {
                    case "1":
                        AddDiscountCard();
                        break;
                    case "2":
                        ViewDiscountCards();
                        break;
                    case "3":
                        UpdateDiscountCard();
                        break;
                    case "4":
                        DeleteDiscountCard();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void AddDiscountCard()
        {
            try
            {
                Console.WriteLine("\nВведите данные скидочной карты:");

                Console.Write("Номер карты: ");
                string cardNumber = Console.ReadLine() ?? "";

                Console.Write("ФИО владельца: ");
                string ownerName = Console.ReadLine() ?? "";

                Console.Write("Процент скидки (0-100): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal discountPercentage) ||
                    discountPercentage < 0 ||
                    discountPercentage > 100)
                {
                    Console.WriteLine("Некорректный процент скидки!");
                    return;
                }

                var discountCard = new DiscountCard
                {
                    CardNumber = cardNumber,
                    OwnerName = ownerName,
                    DiscountPercentage = discountPercentage,
                    IssueDate = DateTime.Now
                };

                _context.DiscountCards.Add(discountCard);
                _context.SaveChanges();
                Console.WriteLine("Скидочная карта успешно добавлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении скидочной карты: {ex.Message}");
            }
        }

        private static void ViewDiscountCards()
        {
            try
            {
                var cards = _context.DiscountCards
                    .OrderBy(c => c.CardNumber)
                    .ToList();

                if (!cards.Any())
                {
                    Console.WriteLine("\nСкидочные карты отсутствуют.");
                    return;
                }

                Console.WriteLine("\nСписок всех скидочных карт:");
                Console.WriteLine(new string('-', 100));
                Console.WriteLine("ID | Номер карты | Владелец | Скидка | Дата выдачи | Кол-во покупок");
                Console.WriteLine(new string('-', 100));

                foreach (var card in cards)
                {
                    int purchaseCount = _context.Sales.Count(s => s.DiscountCardID == card.ID);

                    Console.WriteLine(
                        $"{card.ID,-3} | " +
                        $"{card.CardNumber,-11} | " +
                        $"{card.OwnerName,-20} | " +
                        $"{card.DiscountPercentage,6:F2}% | " +
                        $"{card.IssueDate:dd.MM.yyyy} | " +
                        $"{purchaseCount,14}");
                }

                Console.WriteLine(new string('-', 100));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка скидочных карт: {ex.Message}");
            }
        }

        private static void UpdateDiscountCard()
        {
            try
            {
                Console.Write("\nВведите ID скидочной карты для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var card = _context.DiscountCards.Find(id);
                if (card == null)
                {
                    Console.WriteLine("Скидочная карта не найдена!");
                    return;
                }

                Console.WriteLine("Текущие данные карты:");
                Console.WriteLine($"Номер карты: {card.CardNumber}");
                Console.WriteLine($"Владелец: {card.OwnerName}");
                Console.WriteLine($"Процент скидки: {card.DiscountPercentage}%");
                Console.WriteLine($"Дата выдачи: {card.IssueDate:dd.MM.yyyy}");

                Console.WriteLine("\nОставьте поле пустым, если не хотите его менять.");

                Console.Write("Новый номер карты: ");
                string newCardNumber = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newCardNumber))
                {
                    card.CardNumber = newCardNumber;
                }

                Console.Write("Новый владелец: ");
                string newOwnerName = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newOwnerName))
                {
                    card.OwnerName = newOwnerName;
                }

                Console.Write("Новый процент скидки (0-100): ");
                string discountInput = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(discountInput))
                {
                    if (decimal.TryParse(discountInput, out decimal newDiscountPercentage) &&
                        newDiscountPercentage >= 0 &&
                        newDiscountPercentage <= 100)
                    {
                        card.DiscountPercentage = newDiscountPercentage;
                    }
                    else
                    {
                        Console.WriteLine("Некорректный процент скидки! Это поле не будет обновлено.");
                    }
                }

                _context.SaveChanges();
                Console.WriteLine("Скидочная карта успешно обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении скидочной карты: {ex.Message}");
            }
        }

        private static void DeleteDiscountCard()
        {
            try
            {
                Console.Write("\nВведите ID скидочной карты для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var card = _context.DiscountCards.Find(id);
                if (card == null)
                {
                    Console.WriteLine("Скидочная карта не найдена!");
                    return;
                }

                var hasSales = _context.Sales.Any(s => s.DiscountCardID == id);
                if (hasSales)
                {
                    Console.WriteLine("Внимание! С этой картой связаны продажи.");
                    Console.Write("Вы уверены, что хотите удалить карту? (да/нет): ");
                    if ((Console.ReadLine()?.ToLower() ?? "") != "да")
                    {
                        Console.WriteLine("Операция отменена.");
                        return;
                    }
                }

                _context.DiscountCards.Remove(card);
                _context.SaveChanges();
                Console.WriteLine("Скидочная карта успешно удалена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении скидочной карты: {ex.Message}");
            }
        }

        private static void ManageEmployers()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление сотрудниками ===");
                Console.WriteLine("1. Добавить сотрудника");
                Console.WriteLine("2. Просмотреть всех сотрудников");
                Console.WriteLine("3. Обновить информацию о сотруднике");
                Console.WriteLine("4. Удалить сотрудника");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("\nВыберите опцию: ");

                string opt = Console.ReadLine() ?? "";
                switch (opt)
                {
                    case "1":
                        AddEmployer();
                        break;
                    case "2":
                        ViewEmployers();
                        break;
                    case "3":
                        UpdateEmployer();
                        break;
                    case "4":
                        DeleteEmployer();
                        break;
                    case "0":
                        return;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void AddEmployer()
        {
            try
            {
                Console.WriteLine("\nВведите данные сотрудника:");

                Console.Write("ФИО: ");
                string name = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("ФИО не может быть пустым!");
                    return;
                }

                Console.Write("Должность: ");
                string position = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(position))
                {
                    Console.WriteLine("Должность не может быть пустой!");
                    return;
                }

                var employer = new Employer
                {
                    Name = name,
                    Position = position
                };

                _context.Employers.Add(employer);
                _context.SaveChanges();
                Console.WriteLine("Сотрудник успешно добавлен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при добавлении сотрудника: {ex.Message}");
            }
        }

        private static void ViewEmployers()
        {
            try
            {
                var employers = _context.Employers
                    .Include(e => e.Sales)
                    .OrderBy(e => e.Name)
                    .ToList();

                Console.WriteLine("\nСписок всех сотрудников:");
                foreach (var employer in employers)
                {
                    Console.WriteLine($"ID: {employer.ID}, " +
                                      $"ФИО: {employer.Name}, " +
                                      $"Должность: {employer.Position}, " +
                                      $"Количество продаж: {employer.Sales.Count}");
                }

                Console.WriteLine("\nВведите ID сотрудника для подробной информации или 0 для возврата: ");
                if (int.TryParse(Console.ReadLine(), out int id) && id != 0)
                {
                    var selectedEmployer = employers.FirstOrDefault(e => e.ID == id);
                    if (selectedEmployer != null)
                    {
                        Console.WriteLine($"\nПодробная информация о сотруднике {selectedEmployer.Name}:");
                        Console.WriteLine($"Общая сумма продаж: {selectedEmployer.Sales.Sum(s => s.SalePrice):C}");
                        Console.WriteLine("Последние продажи:");
                        foreach (var sale in selectedEmployer.Sales.OrderByDescending(s => s.SaleDate).Take(5))
                        {
                            Console.WriteLine($"Дата: {sale.SaleDate:d}, Сумма: {sale.SalePrice:C}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Сотрудник не найден!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка сотрудников: {ex.Message}");
            }
        }

        private static void UpdateEmployer()
        {
            try
            {
                Console.Write("\nВведите ID сотрудника для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var employer = _context.Employers.Find(id);
                if (employer == null)
                {
                    Console.WriteLine("Сотрудник не найден!");
                    return;
                }

                Console.WriteLine("Оставьте поле пустым, если не хотите его менять.");

                Console.Write("Новое ФИО: ");
                string newName = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    employer.Name = newName;
                }

                Console.Write("Новая должность: ");
                string newPosition = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newPosition))
                {
                    employer.Position = newPosition;
                }

                _context.SaveChanges();
                Console.WriteLine("Информация о сотруднике успешно обновлена!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении информации о сотруднике: {ex.Message}");
            }
        }

        private static void DeleteEmployer()
        {
            try
            {
                Console.Write("\nВведите ID сотрудника для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var employer = _context.Employers.Find(id);
                if (employer == null)
                {
                    Console.WriteLine("Сотрудник не найден!");
                    return;
                }

                var hasSales = _context.Sales.Any(s => s.SellerID == id);
                if (hasSales)
                {
                    Console.WriteLine("Невозможно удалить сотрудника, так как с ним связаны продажи!");
                    Console.WriteLine("Рекомендуется деактивировать сотрудника вместо удаления.");
                    return;
                }

                _context.Employers.Remove(employer);
                _context.SaveChanges();
                Console.WriteLine("Сотрудник успешно удален!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении сотрудника: {ex.Message}");
            }
        }

        private static void UpdateDisc()
        {
            try
            {
                Console.Write("\nВведите ID диска для обновления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var disc = _context.Discs
                    .Include(d => d.Artist)
                    .FirstOrDefault(d => d.ID == id);

                if (disc == null)
                {
                    Console.WriteLine("Диск не найден!");
                    return;
                }

                Console.WriteLine($"Текущая информация о диске:");
                Console.WriteLine($"Название: {disc.DiscName}");
                Console.WriteLine($"Исполнитель: {disc.Artist?.Name}");
                Console.WriteLine($"Жанр: {disc.Genre}");
                Console.WriteLine($"Цена: {disc.Price:C}");
                Console.WriteLine($"Караоке: {(disc.IsKaraoke ? "Да" : "Нет")}");

                Console.WriteLine("\nОставьте поле пустым, если не хотите его менять.");

                Console.Write("Новое название: ");
                string newName = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newName))
                {
                    disc.DiscName = newName;
                }

                Console.Write("Новый ID исполнителя (текущий: " + disc.ArtistId + "): ");
                string artistIdInput = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(artistIdInput))
                {
                    if (int.TryParse(artistIdInput, out int newArtistId))
                    {
                        var artistExists = _context.Artists.Any(a => a.ID == newArtistId);
                        if (artistExists)
                        {
                            disc.ArtistId = newArtistId;
                        }
                        else
                        {
                            Console.WriteLine("Указанный исполнитель не существует! Это поле не будет обновлено.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ID исполнителя! Это поле не будет обновлено.");
                    }
                }

                Console.Write("Новый жанр: ");
                string newGenre = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(newGenre))
                {
                    disc.Genre = newGenre;
                }

                Console.Write($"Новая цена (текущая: {disc.Price:C}): ");
                string priceInput = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(priceInput))
                {
                    if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice >= 0)
                    {
                        disc.Price = newPrice;
                    }
                    else
                    {
                        Console.WriteLine("Некорректная цена! Это поле не будет обновлено.");
                    }
                }

                Console.Write($"Караоке (true/false) (текущее: {disc.IsKaraoke}): ");
                string karaokeInput = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(karaokeInput))
                {
                    if (bool.TryParse(karaokeInput, out bool newIsKaraoke))
                    {
                        disc.IsKaraoke = newIsKaraoke;
                    }
                    else
                    {
                        Console.WriteLine("Некорректное значение для караоке! Это поле не будет обновлено.");
                    }
                }

                _context.SaveChanges();
                Console.WriteLine("Информация о диске успешно обновлена!");

                Console.WriteLine("\nОбновленная информация о диске:");
                Console.WriteLine($"Название: {disc.DiscName}");
                Console.WriteLine($"Исполнитель: {_context.Artists.Find(disc.ArtistId)?.Name}");
                Console.WriteLine($"Жанр: {disc.Genre}");
                Console.WriteLine($"Цена: {disc.Price:C}");
                Console.WriteLine($"Караоке: {(disc.IsKaraoke ? "Да" : "Нет")}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обновлении информации о диске: {ex.Message}");
            }
        }

        private static void DeleteDisc()
        {
            try
            {
                Console.Write("\nВведите ID диска для удаления: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Некорректный ID!");
                    return;
                }

                var disc = _context.Discs
                    .Include(d => d.Artist)
                    .FirstOrDefault(d => d.ID == id);

                if (disc == null)
                {
                    Console.WriteLine("Диск не найден!");
                    return;
                }

                var hasSales = _context.Sales.Any(s => s.DiscId == id);
                if (hasSales)
                {
                    Console.WriteLine("Невозможно удалить диск, так как с ним связаны продажи!");
                    return;
                }

                Console.WriteLine($"\nВы уверены, что хотите удалить диск:");
                Console.WriteLine($"Название: {disc.DiscName}");
                Console.WriteLine($"Исполнитель: {disc.Artist?.Name}");
                Console.WriteLine($"Цена: {disc.Price:C}");
                Console.Write("\nВведите 'да' для подтверждения: ");

                if ((Console.ReadLine()?.ToLower() ?? "") != "да")
                {
                    Console.WriteLine("Операция отменена.");
                    return;
                }

                _context.Discs.Remove(disc);
                _context.SaveChanges();
                Console.WriteLine("Диск успешно удален!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении диска: {ex.Message}");
            }
        }

        private static void ViewSales()
        {
            try
            {
                var sales = _context.Sales
                    .Include(s => s.SoldDisc)
                    .Include(s => s.Seller)
                    .Include(s => s.DiscountCard)
                    .OrderByDescending(s => s.SaleDate)
                    .ToList();

                if (!sales.Any())
                {
                    Console.WriteLine("\nПродажи отсутствуют.");
                    return;
                }

                Console.WriteLine("\nСписок всех продаж:");
                Console.WriteLine("ID | Дата | Диск | Продавец | Локация | Цена | Скидка");
                Console.WriteLine(new string('-', 80));

                foreach (var sale in sales)
                {
                    string discName = sale.SoldDisc?.DiscName ?? "Неизвестно";
                    string sellerName = sale.Seller?.Name ?? "Неизвестно";
                    string discountInfo = sale.DiscountCard != null
                        ? $"{sale.DiscountCard.DiscountPercentage}%"
                        : "нет";

                    Console.WriteLine(
                        $"{sale.ID} | " +
                        $"{sale.SaleDate:dd.MM.yyyy HH:mm} | " +
                        $"{discName} | " +
                        $"{sellerName} | " +
                        $"{sale.Location} | " +
                        $"{sale.SalePrice:C} | " +
                        $"{discountInfo}");
                }

                decimal totalRevenue = sales.Sum(s => s.SalePrice);
                int totalSales = sales.Count;
                decimal averageSale = totalSales > 0 ? totalRevenue / totalSales : 0;

                Console.WriteLine("\nСтатистика:");
                Console.WriteLine($"Общее количество продаж: {totalSales}");
                Console.WriteLine($"Общая выручка: {totalRevenue:C}");
                Console.WriteLine($"Средний чек: {averageSale:C}");

                Console.Write("\nВведите ID продажи для подробной информации (или 0 для возврата): ");
                if (int.TryParse(Console.ReadLine(), out int saleId) && saleId != 0)
                {
                    var detailedSale = sales.FirstOrDefault(s => s.ID == saleId);
                    if (detailedSale != null)
                    {
                        ShowDetailedSaleInfo(detailedSale);
                    }
                    else
                    {
                        Console.WriteLine("Продажа с указанным ID не найдена.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении списка продаж: {ex.Message}");
            }
        }

        private static void ViewSalesByDate()
        {
            try
            {
                Console.WriteLine("\nПросмотр продаж по дате");
                Console.WriteLine("1. За определенную дату");
                Console.WriteLine("2. За период");
                Console.WriteLine("3. За последние N дней");
                Console.Write("Выберите опцию: ");

                string choice = Console.ReadLine() ?? "";
                switch (choice)
                {
                    case "1":
                        ViewSalesForSpecificDate();
                        break;
                    case "2":
                        ViewSalesForPeriod();
                        break;
                    case "3":
                        ViewSalesForLastDays();
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор!");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при просмотре продаж по дате: {ex.Message}");
            }
        }

        private static void ViewSalesForSpecificDate()
        {
            Console.Write("Введите дату (дд.мм.гггг): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                Console.WriteLine("Некорректный формат даты!");
                return;
            }

            var sales = _context.Sales
                .Include(s => s.SoldDisc)
                .Include(s => s.Seller)
                .Include(s => s.DiscountCard)
                .Where(s => s.SaleDate.Date == date.Date)
                .OrderBy(s => s.SaleDate)
                .ToList();

            DisplaySalesResults(sales, $"Продажи за {date:dd.MM.yyyy}");
        }

        private static void ShowDetailedSaleInfo(Sale sale)
        {
            Console.WriteLine("\nПодробная информация о продаже:");
            Console.WriteLine($"ID продажи: {sale.ID}");
            Console.WriteLine($"Дата и время: {sale.SaleDate:dd.MM.yyyy HH:mm}");

            Console.WriteLine("\nИнформация о диске:");
            Console.WriteLine($"- Название: {sale.SoldDisc?.DiscName ?? "Неизвестно"}");
            Console.WriteLine($"- Исполнитель: {sale.SoldDisc?.Artist?.Name ?? "Неизвестно"}");
            Console.WriteLine($"- Жанр: {sale.SoldDisc?.Genre ?? "Неизвестно"}");

            Console.WriteLine("\nИнформация о продавце:");
            Console.WriteLine($"- ФИО: {sale.Seller?.Name ?? "Неизвестно"}");
            Console.WriteLine($"- Должность: {sale.Seller?.Position ?? "Неизвестно"}");

            Console.WriteLine("\nИнформация о продаже:");
            Console.WriteLine($"- Локация: {sale.Location}");
            Console.WriteLine($"- Цена: {sale.SalePrice:C}");

            if (sale.DiscountCard != null)
            {
                Console.WriteLine("\nИнформация о скидочной карте:");
                Console.WriteLine($"- Номер карты: {sale.DiscountCard.CardNumber}");
                Console.WriteLine($"- Владелец: {sale.DiscountCard.OwnerName}");
                Console.WriteLine($"- Процент скидки: {sale.DiscountCard.DiscountPercentage}%");
            }
        }

        private static void ViewReports()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Отчеты ===");
                Console.WriteLine("1. Отчет по продажам за период");
                Console.WriteLine("2. Топ продаваемых дисков");
                Console.WriteLine("3. Топ исполнителей");
                Console.WriteLine("4. Отчет по сотрудникам");
                Console.WriteLine("5. Финансовый отчет");
                Console.WriteLine("0. Вернуться в главное меню");
                Console.Write("\nВыберите опцию: ");

                string opt = Console.ReadLine() ?? "";
                switch (opt)
                {
                    case "1":
                        SalesByPeriod();
                        break;
                    case "2":
                        TopSellingDiscs();
                        break;
                    case "3":
                        TopSellingArtists();
                        break;
                    case "4":
                        EmployeeReport();
                        break;
                    case "5":
                        FinancialReport();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный выбор!");
                        break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        private static void TopSellingDiscs()
        {
            try
            {
                Console.Write("Введите количество позиций для отображения (по умолчанию 10): ");
                if (!int.TryParse(Console.ReadLine(), out int topCount))
                {
                    topCount = 10;
                }

                var topDiscs = _context.Sales
                    .Include(s => s.SoldDisc)
                    .ThenInclude(d => d.Artist)
                    .GroupBy(s => s.SoldDisc)
                    .Select(g => new
                    {
                        Disc = g.Key,
                        SalesCount = g.Count(),
                        Revenue = g.Sum(s => s.SalePrice)
                    })
                    .OrderByDescending(x => x.SalesCount)
                    .Take(topCount)
                    .ToList();

                Console.WriteLine($"\nТоп {topCount} продаваемых дисков:");
                Console.WriteLine(new string('-', 80));

                int rank = 1;
                foreach (var item in topDiscs)
                {
                    Console.WriteLine($"{rank}. {item.Disc?.DiscName ?? "Неизвестно"} " +
                                      $"(Исполнитель: {item.Disc?.Artist?.Name ?? "Неизвестно"})");
                    Console.WriteLine($"   Продано: {item.SalesCount} шт.");
                    Console.WriteLine($"   Выручка: {item.Revenue:C}");
                    rank++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании отчета: {ex.Message}");
            }
        }

        private static void ViewSalesForPeriod()
        {
            try
            {
                Console.Write("\nВведите начальную дату (дд.мм.гггг): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    Console.WriteLine("Некорректный формат начальной даты!");
                    return;
                }

                Console.Write("Введите конечную дату (дд.мм.гггг): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    Console.WriteLine("Некорректный формат конечной даты!");
                    return;
                }

                if (endDate < startDate)
                {
                    Console.WriteLine("Конечная дата не может быть раньше начальной!");
                    return;
                }

                var sales = _context.Sales
                    .Include(s => s.SoldDisc)
                    .Include(s => s.Seller)
                    .Include(s => s.DiscountCard)
                    .Where(s => s.SaleDate.Date >= startDate.Date && s.SaleDate.Date <= endDate.Date)
                    .OrderBy(s => s.SaleDate)
                    .ToList();

                DisplaySalesResults(sales, $"Продажи за период {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных о продажах за период: {ex.Message}");
            }
        }

        private static void ViewSalesForLastDays()
        {
            try
            {
                Console.Write("\nВведите количество дней: ");
                if (!int.TryParse(Console.ReadLine(), out int days) || days <= 0)
                {
                    Console.WriteLine("Некорректное количество дней!");
                    return;
                }

                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-days);

                var sales = _context.Sales
                    .Include(s => s.SoldDisc)
                    .Include(s => s.Seller)
                    .Include(s => s.DiscountCard)
                    .Where(s => s.SaleDate.Date >= startDate.Date && s.SaleDate.Date <= endDate.Date)
                    .OrderBy(s => s.SaleDate)
                    .ToList();

                DisplaySalesResults(sales,
                    $"Продажи за последние {days} дней ({startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении данных о продажах: {ex.Message}");
            }
        }

        private static void DisplaySalesResults(List<Sale> sales, string headerMessage)
        {
            Console.WriteLine($"\n{headerMessage}");
            Console.WriteLine(new string('-', 100));
            Console.WriteLine(
                "ID | Дата и время | Диск | Продавец | Локация | Цена | Скидка");
            Console.WriteLine(new string('-', 100));

            if (!sales.Any())
            {
                Console.WriteLine("Продажи за указанный период не найдены.");
                return;
            }

            foreach (var sale in sales)
            {
                string discName = sale.SoldDisc?.DiscName ?? "Неизвестно";
                string sellerName = sale.Seller?.Name ?? "Неизвестно";
                string discountInfo = sale.DiscountCard != null
                    ? $"{sale.DiscountCard.DiscountPercentage}%"
                    : "нет";

                Console.WriteLine(
                    $"{sale.ID} | " +
                    $"{sale.SaleDate:dd.MM.yyyy HH:mm} | " +
                    $"{discName} | " +
                    $"{sellerName} | " +
                    $"{sale.Location} | " +
                    $"{sale.SalePrice:C} | " +
                    $"{discountInfo}");
            }

            Console.WriteLine(new string('-', 100));
            Console.WriteLine($"Всего продаж: {sales.Count}");
            Console.WriteLine($"Общая сумма: {sales.Sum(s => s.SalePrice):C}");
        }

        private static void EmployeeReport()
        {
            try
            {
                Console.WriteLine("\n=== Отчет по сотрудникам ===");

                var employeeStats = _context.Sales
                    .Include(s => s.Seller)
                    .Where(s => s.Seller != null)
                    .GroupBy(s => s.Seller)
                    .Select(g => new
                    {
                        Employee = g.Key,
                        TotalSales = g.Count(),
                        TotalRevenue = g.Sum(s => s.SalePrice),
                        AverageRevenue = g.Average(s => s.SalePrice),
                        FirstSale = g.Min(s => s.SaleDate),
                        LastSale = g.Max(s => s.SaleDate)
                    })
                    .OrderByDescending(x => x.TotalRevenue)
                    .ToList();

                if (!employeeStats.Any())
                {
                    Console.WriteLine("Данные о продажах сотрудников отсутствуют.");
                    return;
                }

                Console.WriteLine(new string('-', 120));
                Console.WriteLine(
                    "Сотрудник | Должность | Кол-во продаж | Общая выручка | Средний чек | Первая продажа | Последняя продажа");
                Console.WriteLine(new string('-', 120));

                foreach (var stat in employeeStats)
                {
                    Console.WriteLine(
                        $"{stat.Employee.Name,-10} | " +
                        $"{stat.Employee.Position,-10} | " +
                        $"{stat.TotalSales,12} | " +
                        $"{stat.TotalRevenue,13:C} | " +
                        $"{stat.AverageRevenue,10:C} | " +
                        $"{stat.FirstSale,13:dd.MM.yyyy} | " +
                        $"{stat.LastSale:dd.MM.yyyy}");
                }

                Console.WriteLine(new string('-', 120));
                Console.WriteLine($"Общее количество сотрудников: {employeeStats.Count}");
                Console.WriteLine($"Общая выручка всех сотрудников: {employeeStats.Sum(x => x.TotalRevenue):C}");
                Console.WriteLine(
                    $"Среднее количество продаж на сотрудника: {employeeStats.Average(x => x.TotalSales):F2}");

                var bestEmployee = employeeStats.OrderByDescending(x => x.TotalRevenue).First();
                Console.WriteLine($"\nЛучший сотрудник по выручке: {bestEmployee.Employee.Name}");
                Console.WriteLine($"Общая выручка: {bestEmployee.TotalRevenue:C}");
                Console.WriteLine($"Количество продаж: {bestEmployee.TotalSales}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании отчета по сотрудникам: {ex.Message}");
            }
        }

        private static void FinancialReport()
        {
            try
            {
                Console.WriteLine("\n=== Финансовый отчет ===");

                var sales = _context.Sales
                    .Include(s => s.SoldDisc)
                    .Include(s => s.Seller)
                    .Include(s => s.DiscountCard)
                    .ToList();

                if (!sales.Any())
                {
                    Console.WriteLine("Данные о продажах отсутствуют.");
                    return;
                }

                var totalRevenue = sales.Sum(s => s.SalePrice);
                var totalCost = sales.Sum(s => s.SoldDisc?.PurchasePrice ?? 0);
                var totalProfit = totalRevenue - totalCost;
                var averageProfit = totalProfit / sales.Count;

                var monthlyStats = sales
                    .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Revenue = g.Sum(s => s.SalePrice),
                        Cost = g.Sum(s => s.SoldDisc?.PurchasePrice ?? 0),
                        SalesCount = g.Count()
                    })
                    .OrderByDescending(x => x.Year)
                    .ThenByDescending(x => x.Month)
                    .ToList();

                Console.WriteLine("\n=== Общая статистика ===");
                Console.WriteLine($"Общая выручка: {totalRevenue:C}");
                Console.WriteLine($"Общие затраты: {totalCost:C}");
                Console.WriteLine($"Общая прибыль: {totalProfit:C}");
                Console.WriteLine($"Средняя прибыль с продажи: {averageProfit:C}");
                Console.WriteLine($"Количество продаж: {sales.Count}");

                var discountStats = sales
                    .Where(s => s.DiscountCard != null)
                    .GroupBy(s => s.DiscountCard.DiscountPercentage)
                    .Select(g => new
                    {
                        DiscountPercentage = g.Key,
                        Count = g.Count(),
                        TotalDiscount = g.Sum(s => s.SalePrice * g.Key / 100)
                    })
                    .OrderByDescending(x => x.Count);

                Console.WriteLine("\n=== Статистика по скидкам ===");
                foreach (var stat in discountStats)
                {
                    Console.WriteLine(
                        $"Скидка {stat.DiscountPercentage}%: {stat.Count} продаж, сумма скидок: {stat.TotalDiscount:C}");
                }

                Console.WriteLine("\n=== Статистика по месяцам ===");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine("Период | Выручка | Затраты | Прибыль | Кол-во продаж | Средний чек");
                Console.WriteLine(new string('-', 80));

                foreach (var stat in monthlyStats)
                {
                    var profit = stat.Revenue - stat.Cost;
                    var averageCheck = stat.Revenue / stat.SalesCount;

                    Console.WriteLine(
                        $"{stat.Year}/{stat.Month:00} | " +
                        $"{stat.Revenue,8:C} | " +
                        $"{stat.Cost,7:C} | " +
                        $"{profit,8:C} | " +
                        $"{stat.SalesCount,13} | " +
                        $"{averageCheck,11:C}");
                }

                if (monthlyStats.Count >= 2)
                {
                    var lastMonth = monthlyStats.First();
                    var previousMonth = monthlyStats.Skip(1).First();
                    var revenueChange = ((lastMonth.Revenue - previousMonth.Revenue) / previousMonth.Revenue) * 100;

                    Console.WriteLine("\n=== Анализ трендов ===");
                    Console.WriteLine($"Изменение выручки к прошлому месяцу: {revenueChange:F2}%");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при формировании финансового отчета: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
    }
}