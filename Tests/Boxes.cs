namespace Test;
using System.Reflection;

[TestFixture]
public class Boxes
{

    [Test]
    public void AddFewTakeFew()
    {
        var box = new Box();
        box.TryStore(0, 1);
        var taken = box.Take(0, 1);
        // taken must be 1 of the type 0
        Assert.AreEqual(1, taken.Count);
        Assert.AreEqual(0, taken.ItemId);
    }
    [Test]
    public void AddFewTakeMany()
    {
        var box = new Box();
        box.TryStore(0, 1);
        var taken = box.Take(0, 10);
        // taken must be 1 of the type 0
        Assert.AreEqual(1, taken.Count);
        Assert.AreEqual(0, taken.ItemId);
    }

    [Test]
    public void AddItemIntoAZeroCapacityBox()
    {
        var box = new Box { Capacity = 0 };
        Assert.IsFalse(box.TryStore(1, 1));
    }

    [Test]
    public void AddManyTakeFew()
    {
        var box = new Box();
        box.TryStore(0, 10);
        var taken = box.Take(0, 2);
        // taken must be 2 of the type 0
        Assert.AreEqual(2, taken.Count);
        Assert.AreEqual(0, taken.ItemId);
    }
    [Test]
    public void AddThenGetByIndexer()
    {
        var box = new Box();
        box.TryStore(0, 1);
        Assert.AreEqual(1, box[0]);
    }

    [Test]
    public void AddingMoreThatCapacityMustNotChangeTheContent()
    {
        var box = new Box { Capacity = 10 };
        var added = box.TryStore(1, 20);
        // The box must be empty
        Assert.IsFalse(added);
        Assert.AreEqual(0, box.UsedCapacity);

        // Use reflection to get the items field and check if it is empty
        var itemsField = typeof(Box).GetField("items", BindingFlags.Instance | BindingFlags.NonPublic);
        var items = (Dictionary<int, int>)itemsField!.GetValue(box)!;
        // We don't know whether the items dictionary is empty or not, but the sum of the values must be 0
        Assert.AreEqual(0, items.Sum(i => i.Value));
    }

    [Test]
    public void AddingNegativeAmountMustThrow()
    {
        var box = new Box { Capacity = 10 };
        Assert.Throws<InvalidOperationException>(() => box.TryStore(1, -1));
    }

    [Test]
    public void AddingUpdateFreeCapacity()
    {
        var box = new Box { Capacity = 10 };
        box.TryStore(1, 1);
        Assert.AreEqual(9, box.FreeCapacity);
        Assert.AreEqual(1, box.UsedCapacity);

        box.TryStore(1, 2);
        Assert.AreEqual(7, box.FreeCapacity);
        Assert.AreEqual(3, box.UsedCapacity);
    }

    [Test]
    public void NegativeCapacityBoxesMustNotExist()
    {
        Assert.Throws<InvalidOperationException>(() => new Box { Capacity = -1 });
    }

    [Test]
    public void RemovingMultipleItemsNotEnoughItemsToRemove()
    {
        var box = new Box{Capacity = 10};
        // Add items of two different ids
        box.TryStore(0, 3);
        box.TryStore(1, 3);

        // create a span of items to remove, in form of stacks
        var items = new ItemStack[2]{
            new ItemStack{ItemId = 0, Count = 1},
            new ItemStack{ItemId = 1, Count = 4}
        };
        // remove the items
        var removed = box.TryRemoveItems(items);

        // removed must be false
        Assert.AreEqual(false, removed);
        // the new content must be the same as before
        Assert.AreEqual(3, box[0]);
        Assert.AreEqual(3, box[1]);
    }

    [Test]
    public void RemovingMultipleItemsOk()
    {
        var box = new Box();
        // Add items of two different ids
        box.TryStore(0, 3);
        box.TryStore(1, 3);

        // create a span of items to remove, in form of stacks
        var items = new ItemStack[2]{
            new ItemStack{ItemId = 0, Count = 1},
            new ItemStack{ItemId = 1, Count = 2}
        };
        // remove the items
        var removed = box.TryRemoveItems(items);

        // removed must be true
        Assert.AreEqual(true, removed);
        // the new content must be 2 of the type 0 and 1 of the type 1
        Assert.AreEqual(2, box[0]);
        Assert.AreEqual(1, box[1]);
    }

    [Test]
    public void StoreAsMuch_StoreAll()
    {
        var box = new Box { Capacity = 100 };
        // create an array of stacks to store, in form of stacks
        // Store 20 items of type 0
        box.TryStoreAsMuchAsPossible(0, 20);
        // Check content
        Assert.AreEqual(20, box[0]);
        Assert.AreEqual(20, box.UsedCapacity);
        Assert.AreEqual(80, box.FreeCapacity);
    }

    [Test]
    public void StoreAsMuch_StorePossible()
    {
        var box = new Box { Capacity = 10 };
        // create an array of stacks to store, in form of stacks
        // Store 20 items of type 0
        box.TryStoreAsMuchAsPossible(0, 20);
        // Check content
        Assert.AreEqual(10, box[0]);
        Assert.AreEqual(10, box.UsedCapacity);
        Assert.AreEqual(0, box.FreeCapacity);
    }

    [Test]
    public void StoreFromSpanNoCapacity()
    {
        var box = new Box { Capacity = 2 };
        // create an array of stacks to store, in form of stacks
        var items = new ItemStack[2]{
            new ItemStack{ItemId = 0, Count = 1},
            new ItemStack{ItemId = 1, Count = 2}
        };

        // store the items
        var added = box.TryStore(items);
        // added must be false
        Assert.AreEqual(false, added);
        // the box must be emtpty
        Assert.AreEqual(0, box.UsedCapacity);
    }

    [Test]
    public void StoreFromSpanOk()
    {
        var box = new Box();
        // create an array of stacks to store, in form of stacks
        var items = new ItemStack[2]{
            new ItemStack{ItemId = 0, Count = 1},
            new ItemStack{ItemId = 1, Count = 2}
        };

        // store the items
        var added = box.TryStore(items);
        // added must be true
        Assert.AreEqual(true, added);
        // the new content must be 1 of the type 0 and 2 of the type 1
        Assert.AreEqual(1, box[0]);
        Assert.AreEqual(2, box[1]);
    }

    [Test]
    public void TakeMethodMustTakeOfTheRightType()
    {
        var box = new Box();
        // Add items of two different ids
        box.TryStore(0, 3);
        box.TryStore(1, 3);
        // Check content
        Assert.AreEqual(3, box[0]);
        Assert.AreEqual(3, box[1]);

        // Take items of one type
        var taken = box.Take(0, 1);
        // Check content
        Assert.AreEqual(2, box[0]);
        Assert.AreEqual(3, box[1]);

        // Take items of another type
        taken = box.Take(1, 2);
        // Check content
        Assert.AreEqual(2, box[0]);
        Assert.AreEqual(1, box[1]);
    }
}