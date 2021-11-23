// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");



/*
protected override void ConfigureConventions (ModelConfigurationBuilder configurationBuilder)
{
	configurationBuilder.Properties<string>().HaveMaxLength(200); //Применяет настройки ко всем строкам если нетдругих переметров в моделбилдер или аттрибутов
}

dotnet ef migrations bundle // создает exe с миграциями и применяет их   можно доп параметрами раскатьна любую бд прод тест то се

modelBuilder.Entity<Customer>().ToTable(t=>t.IsTemporal()); //временная таблица

context.Customers.TemporalAll()  //Смотреть всю историю временной таблицы

context.Customers.TemporalFromTo(<from>,<to>) //От и до изменения

context.Customers.TemporalAsOf(<on>) //Изменения в это время

[Аттрибут на точность  decimal]
*/