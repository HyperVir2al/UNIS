using Universities.Data;
using Universities.Queries;

Console.Title = "LINQ Template - WW2 Tanks Edition";

using (var db = new UnisContext())
{
    Console.WriteLine("Инициализация базы данных SQLite (техника Второй мировой войны)...");
    DbInitializer.Initialize(db);
    Console.WriteLine("База данных готова к работе!");

    LinqDemos.DemoFilteringAndSorting(db);
    LinqDemos.DemoProjection(db);
    LinqDemos.DemoAggregations(db);
    LinqDemos.DemoGrouping(db);
    LinqDemos.DemoJoinsAndIncludes(db);
}

Console.WriteLine("\n\nНажмите любую клавишу для завершения...");
Console.ReadKey();