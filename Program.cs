// See https://aka.ms/new-console-template for more information

var factory = new FurnitureFactorySection();

var oakСarpenter = new OakСarpenter();
var pineСarpenter = new PineСarpenter();

var furnitures = new Furniture[3];

furnitures[0] = await factory.BuildFurnitureAsync<Table>(pineСarpenter);
furnitures[1] = await factory.BuildFurnitureAsync<Chair>(pineСarpenter);
furnitures[2] = await factory.BuildFurnitureAsync<Chair>(oakСarpenter);

foreach (var item in furnitures)
{
    Console.WriteLine($"Мебель - {item.GetType()}, покрашена - {(item.IsAllPainted ? "да" : "нет")}" +
        $", отполирована - {(item.IsAllPolished ? "да" : "нет")}" +
        $", покарыта лаком - {(item.IsAllVarnished ? "да" : "нет")}");
}

Console.ReadLine();

[Flags]
public enum WoodProperty : byte
{
    None = 0,
    Painted= 1 << 0,
    Polished = 1 << 1,
    Varnished = 1 << 2

}

public interface IMakesholes
{
    public bool IsConnected { get; set; }
}
public abstract class Plank: IMakesholes
{
    public required int Length { get; init; }
    public  WoodProperty Property { get; set; }
    public bool IsConnected { get; set; }
}

public class PinePlank: Plank
{

}

public class OakPlank : Plank
{

}


public abstract class Furniture
{

    public IEnumerable<Plank> Planks { get; init; }
    public IBuildingElementConnector BuildingElementConnector { get; init; }
    public bool IsAllPainted { get; set; }
    public bool IsAllPolished { get; set; }
    public bool IsAllVarnished { get; set; }

}

public class Table : Furniture
{

}

public class Chair : Furniture
{

}

class FurnitureFactorySection
{

    public async ValueTask<Furniture> BuildFurnitureAsync<T>(Сarpenter сarpenter) where T : Furniture, new()
    {
        await сarpenter.CreateFurnitureAsync<T>();
        сarpenter.SetPolish();
        сarpenter.SetPaint();
        сarpenter.SetVarnish();
        return сarpenter.Furniture;
    }

}


public abstract class Сarpenter
{
    public Furniture Furniture { get;  set; }
    public abstract void SetPaint();
    public abstract void SetPolish();
    public abstract void SetVarnish();
    public abstract ValueTask CreateFurnitureAsync<T>() where T : Furniture, new();


}

 public class OakСarpenter : Сarpenter
{

    public override async ValueTask CreateFurnitureAsync<T>() 
    {

        var t = typeof(T);
        var nails = new Nails();
        var planks = new List<Plank>();
        switch (true)
        {
            case var _ when t.IsAssignableFrom(typeof(Table)):

                planks.Add(new OakPlank() { Length = 5 });
                planks.Add(new OakPlank() { Length = 5 });
                planks.Add(new OakPlank() { Length = 5 });
                planks.Add(new OakPlank() { Length = 5 });
                planks.Add(new OakPlank() { Length = 10 });
                break;

            case var _ when t.IsAssignableFrom(typeof(Chair)):
                planks.Add(new OakPlank() { Length = 15 });
                planks.Add(new OakPlank() { Length = 15 });
                planks.Add(new OakPlank() { Length = 10 });
                planks.Add(new OakPlank() { Length = 10 });
                break;

            default:
                 throw new NotSupportedException();
        }
        foreach (var plank in planks)
        {
            nails.ConnectElement(plank);
        }
        Furniture = new T() { Planks = planks, BuildingElementConnector = nails };
        await Task.Delay(500);
    }

    public override void SetPaint()
    {
        if (Furniture != null)
        {
            foreach (var item in Furniture.Planks)
            {
                item.Property = WoodProperty.Painted;
            }
            Furniture.IsAllPainted = true;
        }
    }

    public override void SetPolish()
    {
        if (Furniture != null)
        {
            foreach (var item in Furniture.Planks)
            {
                item.Property = WoodProperty.Polished;
            }
            Furniture.IsAllPolished = true;
        }
    }

    public override void SetVarnish()
    {
        if (Furniture != null)
        {
            foreach (var item in Furniture.Planks)
            {
                item.Property = WoodProperty.Varnished;
            }
            Furniture.IsAllVarnished = true;
        }
    }
}


public class PineСarpenter : Сarpenter
{
    public override async ValueTask CreateFurnitureAsync<T>()
    {
        var t = typeof(T);
        var nails = new SelfTappingScrews();
        var planks = new List<Plank>();
        switch (true)
        {
            case var _ when t.IsAssignableFrom(typeof(Table)):

                planks.Add(new PinePlank() { Length = 5 });
                planks.Add(new PinePlank() { Length = 5 });
                planks.Add(new PinePlank() { Length = 5 });
                planks.Add(new PinePlank() { Length = 5 });
                planks.Add(new PinePlank() { Length = 10 });
                break;

            case var _ when t.IsAssignableFrom(typeof(Chair)):
                planks.Add(new PinePlank() { Length = 15 });
                planks.Add(new PinePlank() { Length = 15 });
                planks.Add(new PinePlank() { Length = 10 });
                planks.Add(new PinePlank() { Length = 10 });
                break;

            default:
                throw new NotSupportedException();
        }
        foreach (var plank in planks)
        {
            nails.ConnectElement(plank);
        }
        Furniture = new T() { Planks = planks, BuildingElementConnector = nails };
        await Task.Delay(500);
    }

    public override void SetPaint()
    {
        if (Furniture != null)
        {
            foreach (var item in Furniture.Planks)
            {
                item.Property = WoodProperty.Painted;
            }
            Furniture.IsAllPainted = true;
        }
    }

    public override void SetPolish()
    {
        if (Furniture != null)
        {
            Console.WriteLine("Не умею");
        }
    }

    public override void SetVarnish()
    {
        if (Furniture != null)
        {
            foreach (var item in Furniture.Planks)
            {
                item.Property = WoodProperty.Varnished;
            }
            Furniture.IsAllVarnished = true;
        }
    }
}

public interface IBuildingElementConnector
{
    public void ConnectElement(IMakesholes makesholes); 
}

public class Nails : IBuildingElementConnector
{
    public void ConnectElement(IMakesholes makesholes)
    {
        makesholes.IsConnected = true;
        Console.WriteLine("Соеденяем с помощью гвоздей");
    }
}
public class SelfTappingScrews: IBuildingElementConnector
{
    public void ConnectElement(IMakesholes makesholes)
    {
        makesholes.IsConnected = true;
        Console.WriteLine("Соеденяем с помощью саморезов");
    }
}