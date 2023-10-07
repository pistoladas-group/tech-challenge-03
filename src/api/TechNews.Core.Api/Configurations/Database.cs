using Microsoft.EntityFrameworkCore;
using TechNews.Common.Library.Extensions;
using TechNews.Core.Api.Data;
using TechNews.Core.Api.Data.Models;

namespace TechNews.Core.Api.Configurations;

public static class Database
{
    public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
    {
        var connectionString = EnvironmentVariables.DatabaseConnectionString;

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ApplicationException("Undefined Database Connection String. Please check the Environment Variables.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString, assembly =>
                assembly.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                        .EnableRetryOnFailure(maxRetryCount: 2, maxRetryDelay: TimeSpan.FromSeconds(2), errorNumbersToAdd: null)
                        .MigrationsHistoryTable("_AppliedMigrations"));
        });

        return services;
    }

    public static void MigrateDatabase(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();

        dbContext.RemoveRange(dbContext.News.ToList());
        dbContext.RemoveRange(dbContext.Authors.ToList());

        var authorsData = new List<Author>
        {
            new(name: "Everton Alves", email: "everton.alves@teste.com", imageSource: "https://picsum.photos/seed/1/60"),
            new(name: "Igor Moura", email: "igor.moura@teste.com", imageSource: "https://picsum.photos/seed/2/60"),
            new(name: "Danilo Vasconcellos", email: "danilo.vasconcellos@teste.com", imageSource: "https://picsum.photos/seed/3/60")
        };

        var newsData = new List<News>
        {
            new(title: "Suco de cevadiss deixa as pessoas mais interessantis", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Suco de cevadiss deixa as pessoas mais interessantis. Casamentiss faiz malandris se pirulitá. Praesent malesuada urna nisi, quis volutpat erat hendrerit non. Nam vulputate dapibus. Copo furadis é disculpa de bebadis, arcu quam euismod magna.
Suco de cevadiss deixa as pessoas mais interessantis. Leite de capivaris, leite de mula manquis sem cabeça. Quem num gosta di mim que vai caçá sua turmis! Manduma pindureta quium dia nois paga.
Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Quem num gosta di mim que vai caçá sua turmis! Cevadis im ampola pa arma uma pindureta. Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo!
Nec orci ornare consequat. Praesent lacinia ultrices consectetur. Sed non ipsum felis. Pra lá, depois divoltis porris, paradis. Admodum accumsan disputationi eu sit. Vide electram sadipscing et per. Quem num gosta di mim que vai caçá sua turmis!
Interagi no mé, cursus quis, vehicula ac nisi. Leite de capivaris, leite de mula manquis sem cabeça. Aenean aliquam molestie leo, vitae iaculis nisl. Viva Forevis aptent taciti sociosqu ad litora torquent.
Leite de capivaris, leite de mula manquis sem cabeça. Mauris nec dolor in eros commodo tempor. Aenean aliquam molestie leo, vitae iaculis nisl. Paisis, filhis, espiritis santis. Sapien in monti palavris qui num significa nadis i pareci latim.", publishDate: new DateTime(2023, 08, 01, 11, 30, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/4"),
            new(title: "Não sou faixa preta cumpadi, sou preto inteiris", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Quem num gosta di mim que vai caçá sua turmis! Praesent vel viverra nisi. Mauris aliquet nunc non turpis scelerisque, eget. Admodum accumsan disputationi eu sit. Vide electram sadipscing et per.
Paisis, filhis, espiritis santis. Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Atirei o pau no gatis, per gatis num morreus. Si num tem leite então bota uma pinga aí cumpadi!
Em pé sem cair, deitado sem dormir, sentado sem cochilar e fazendo pose. In elementis mé pra quem é amistosis quis leo. Sapien in monti palavris qui num significa nadis i pareci latim. Copo furadis é disculpa de bebadis, arcu quam euismod magna.
In elementis mé pra quem é amistosis quis leo. Atirei o pau no gatis, per gatis num morreus. Diuretics paradis num copo é motivis de denguis. Per aumento de cachacis, eu reclamis.
Per aumento de cachacis, eu reclamis. Mé faiz elementum girarzis, nisi eros vermeio. Pra lá, depois divoltis porris, paradis. Suco de cevadiss deixa as pessoas mais interessantis.
In elementis mé pra quem é amistosis quis leo. Quem manda na minha terra sou euzis! Admodum accumsan disputationi eu sit. Vide electram sadipscing et per. Atirei o pau no gatis, per gatis num morreus.", publishDate: new DateTime(2023, 09, 02, 10, 40, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/5"),
            new(title: "Per aumento de cachacis, eu reclamis", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Per aumento de cachacis, eu reclamis. Admodum accumsan disputationi eu sit. Vide electram sadipscing et per. Cevadis im ampola pa arma uma pindureta. Nullam volutpat risus nec leo commodo, ut interdum diam laoreet. Sed non consequat odio.
Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi. Per aumento de cachacis, eu reclamis. Quem num gosta di mé, boa gentis num é. Mauris nec dolor in eros commodo tempor. Aenean aliquam molestie leo, vitae iaculis nisl.
Pra lá, depois divoltis porris, paradis. A ordem dos tratores não altera o pão duris. Copo furadis é disculpa de bebadis, arcu quam euismod magna. Quem num gosta di mim que vai caçá sua turmis!
Interessantiss quisso pudia ce receita de bolis, mais bolis eu num gostis. Sapien in monti palavris qui num significa nadis i pareci latim. Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi. Quem num gosta di mé, boa gentis num é.
Leite de capivaris, leite de mula manquis sem cabeça. In elementis mé pra quem é amistosis quis leo. Si num tem leite então bota uma pinga aí cumpadi! Mais vale um bebadis conhecidiss, que um alcoolatra anonimis.
Mais vale um bebadis conhecidiss, que um alcoolatra anonimis. Nec orci ornare consequat. Praesent lacinia ultrices consectetur. Sed non ipsum felis. Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo! Per aumento de cachacis, eu reclamis.", publishDate: new DateTime(2023, 10, 01, 09, 15, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/6"),
            new(title: "A ordem dos tratores não altera o pão duris", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Mauris nec dolor in eros commodo tempor. Aenean aliquam molestie leo, vitae iaculis nisl. Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi. Mais vale um bebadis conhecidiss, que um alcoolatra anonimis. Viva Forevis aptent taciti sociosqu ad litora torquent.
In elementis mé pra quem é amistosis quis leo. A ordem dos tratores não altera o pão duris. Nec orci ornare consequat. Praesent lacinia ultrices consectetur. Sed non ipsum felis. Interessantiss quisso pudia ce receita de bolis, mais bolis eu num gostis.
Mé faiz elementum girarzis, nisi eros vermeio. Delegadis gente finis, bibendum egestas augue arcu ut est. Cevadis im ampola pa arma uma pindureta. Admodum accumsan disputationi eu sit. Vide electram sadipscing et per.
In elementis mé pra quem é amistosis quis leo. Per aumento de cachacis, eu reclamis. Leite de capivaris, leite de mula manquis sem cabeça. Atirei o pau no gatis, per gatis num morreus.
Quem num gosta di mé, boa gentis num é. Per aumento de cachacis, eu reclamis. Admodum accumsan disputationi eu sit. Vide electram sadipscing et per. Nullam volutpat risus nec leo commodo, ut interdum diam laoreet. Sed non consequat odio.
Per aumento de cachacis, eu reclamis. Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo! Paisis, filhis, espiritis santis. Casamentiss faiz malandris se pirulitá.", publishDate: new DateTime(2023, 07, 04, 03, 00, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/7"),
            new(title: "Paisis, filhis, espiritis santis", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Paisis, filhis, espiritis santis. Quem num gosta di mim que vai caçá sua turmis! Delegadis gente finis, bibendum egestas augue arcu ut est. Mais vale um bebadis conhecidiss, que um alcoolatra anonimis.
Vehicula non. Ut sed ex eros. Vivamus sit amet nibh non tellus tristique interdum. Pra lá, depois divoltis porris, paradis. Per aumento de cachacis, eu reclamis. Manduma pindureta quium dia nois paga.
Copo furadis é disculpa de bebadis, arcu quam euismod magna. Si num tem leite então bota uma pinga aí cumpadi! Aenean aliquam molestie leo, vitae iaculis nisl. Sapien in monti palavris qui num significa nadis i pareci latim.
Interessantiss quisso pudia ce receita de bolis, mais bolis eu num gostis. Detraxit consequat et quo num tendi nada. Si num tem leite então bota uma pinga aí cumpadi! Praesent malesuada urna nisi, quis volutpat erat hendrerit non. Nam vulputate dapibus.
Mais vale um bebadis conhecidiss, que um alcoolatra anonimis. Em pé sem cair, deitado sem dormir, sentado sem cochilar e fazendo pose. Leite de capivaris, leite de mula manquis sem cabeça. Manduma pindureta quium dia nois paga.
Paisis, filhis, espiritis santis. Em pé sem cair, deitado sem dormir, sentado sem cochilar e fazendo pose. Praesent malesuada urna nisi, quis volutpat erat hendrerit non. Nam vulputate dapibus. In elementis mé pra quem é amistosis quis leo.", publishDate: new DateTime(2023, 07, 06, 06, 06, 06), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/8"),
            new(title: "Mé faiz elementum girarzis, nisi eros vermeio", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Mé faiz elementum girarzis, nisi eros vermeio. Interagi no mé, cursus quis, vehicula ac nisi. Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi.
Quem num gosta di mim que vai caçá sua turmis! Quem manda na minha terra sou euzis! Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo! Copo furadis é disculpa de bebadis, arcu quam euismod magna.
Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Si u mundo tá muito paradis? Toma um mé que o mundo vai girarzis! Nullam volutpat risus nec leo commodo, ut interdum diam laoreet. Sed non consequat odio. Praesent vel viverra nisi. Mauris aliquet nunc non turpis scelerisque, eget.
A ordem dos tratores não altera o pão duris. Viva Forevis aptent taciti sociosqu ad litora torquent. Interessantiss quisso pudia ce receita de bolis, mais bolis eu num gostis. Interagi no mé, cursus quis, vehicula ac nisi.
Leite de capivaris, leite de mula manquis sem cabeça. Interessantiss quisso pudia ce receita de bolis, mais bolis eu num gostis. Detraxit consequat et quo num tendi nada. Mé faiz elementum girarzis, nisi eros vermeio.", publishDate: new DateTime(2023, 06, 01, 10, 00, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/9"),
            new(title: "Mais vale um bebadis conhecidiss, que um alcoolatra anonimis", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Mais vale um bebadis conhecidiss, que um alcoolatra anonimis. Suco de cevadiss, é um leite divinis, qui tem lupuliz, matis, aguis e fermentis. Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Cevadis im ampola pa arma uma pindureta.
Paisis, filhis, espiritis santis. Quem manda na minha terra sou euzis! Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi. Suco de cevadiss, é um leite divinis, qui tem lupuliz, matis, aguis e fermentis.
Em pé sem cair, deitado sem dormir, sentado sem cochilar e fazendo pose. Quem num gosta di mim que vai caçá sua turmis! Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Diuretics paradis num copo é motivis de denguis.
Manduma pindureta quium dia nois paga. Em pé sem cair, deitado sem dormir, sentado sem cochilar e fazendo pose. Per aumento de cachacis, eu reclamis. Suco de cevadiss deixa as pessoas mais interessantis.
Viva Forevis aptent taciti sociosqu ad litora torquent. Si u mundo tá muito paradis? Toma um mé que o mundo vai girarzis! Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi. Não sou faixa preta cumpadi, sou preto inteiris, inteiris.", publishDate: new DateTime(2011, 11, 11, 11, 11, 11), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/10"),
            new(title: "Quem num gosta di mé, boa gentis num é", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Quem num gosta di mé, boa gentis num é. Mauris nec dolor in eros commodo tempor. Aenean aliquam molestie leo, vitae iaculis nisl. Em pé sem cair, deitado sem dormir, sentado sem cochilar e fazendo pose. Vehicula non. Ut sed ex eros. Vivamus sit amet nibh non tellus tristique interdum.
Copo furadis é disculpa de bebadis, arcu quam euismod magna. Vehicula non. Ut sed ex eros. Vivamus sit amet nibh non tellus tristique interdum. Sapien in monti palavris qui num significa nadis i pareci latim. Detraxit consequat et quo num tendi nada.
Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Mé faiz elementum girarzis, nisi eros vermeio. Suco de cevadiss, é um leite divinis, qui tem lupuliz, matis, aguis e fermentis. Manduma pindureta quium dia nois paga.
Diuretics paradis num copo é motivis de denguis. Mé faiz elementum girarzis, nisi eros vermeio. Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo! Quem manda na minha terra sou euzis!
Casamentiss faiz malandris se pirulitá. Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Detraxit consequat et quo num tendi nada. Manduma pindureta quium dia nois paga.", publishDate: new DateTime(2023, 09, 11, 10, 30, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/11"),
            new(title: "Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Suco de cevadiss deixa as pessoas mais interessantis. Quem num gosta di mim que vai caçá sua turmis! Mé faiz elementum girarzis, nisi eros vermeio. Praesent malesuada urna nisi, quis volutpat erat hendrerit non. Nam vulputate dapibus.
Praesent vel viverra nisi. Mauris aliquet nunc non turpis scelerisque, eget. Quem manda na minha terra sou euzis! Sapien in monti palavris qui num significa nadis i pareci latim. Delegadis gente finis, bibendum egestas augue arcu ut est.
Mauris nec dolor in eros commodo tempor. Aenean aliquam molestie leo, vitae iaculis nisl. Todo mundo vê os porris que eu tomo, mas ninguém vê os tombis que eu levo! Interagi no mé, cursus quis, vehicula ac nisi. Leite de capivaris, leite de mula manquis sem cabeça.
Aenean aliquam molestie leo, vitae iaculis nisl. Nec orci ornare consequat. Praesent lacinia ultrices consectetur. Sed non ipsum felis. Vehicula non. Ut sed ex eros. Vivamus sit amet nibh non tellus tristique interdum. Não sou faixa preta cumpadi, sou preto inteiris, inteiris.
Sapien in monti palavris qui num significa nadis i pareci latim. Viva Forevis aptent taciti sociosqu ad litora torquent. Suco de cevadiss deixa as pessoas mais interessantis. Copo furadis é disculpa de bebadis, arcu quam euismod magna.", publishDate: new DateTime(2023, 04, 30, 12, 15, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/12"),
            new(title: "Atirei o pau no gatis, per gatis num morreus", description: @"Mussum Ipsum, cacilds vidis litro abertis.  Aenean aliquam molestie leo, vitae iaculis nisl. Atirei o pau no gatis, per gatis num morreus. Não sou faixa preta cumpadi, sou preto inteiris, inteiris. Detraxit consequat et quo num tendi nada.
Pra lá, depois divoltis porris, paradis. Copo furadis é disculpa de bebadis, arcu quam euismod magna. Diuretics paradis num copo é motivis de denguis. Per aumento de cachacis, eu reclamis.
In elementis mé pra quem é amistosis quis leo. Sapien in monti palavris qui num significa nadis i pareci latim. Copo furadis é disculpa de bebadis, arcu quam euismod magna. Suco de cevadiss, é um leite divinis, qui tem lupuliz, matis, aguis e fermentis.
Suco de cevadiss deixa as pessoas mais interessantis. Tá deprimidis, eu conheço uma cachacis que pode alegrar sua vidis. Mé faiz elementum girarzis, nisi eros vermeio. Admodum accumsan disputationi eu sit. Vide electram sadipscing et per.
Si u mundo tá muito paradis? Toma um mé que o mundo vai girarzis! Posuere libero varius. Nullam a nisl ut ante blandit hendrerit. Aenean sit amet nisi. Copo furadis é disculpa de bebadis, arcu quam euismod magna. Sapien in monti palavris qui num significa nadis i pareci latim.", publishDate: new DateTime(2023, 01, 25, 08, 40, 00), author: authorsData.Random(), imageSource: "https://picsum.photos/seed/13"),
        };

        dbContext.Authors.AddRange(authorsData);
        dbContext.News.AddRange(newsData);

        dbContext.SaveChanges();
    }
}