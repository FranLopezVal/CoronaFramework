using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corona
{
    public class Inventory
    {
        private int _id;

        public List<c_Item> items;
        public int maxInventorySize;

        public Inventory()
        {
            _id = Corona.FindSystem<c_InventorySystem>().GetLastIndex();
        }

        public int GetId() => _id;
        public List<c_Item> AllItems => items;

        public bool AddItem(c_Item item)
        {
            if (items.Count < maxInventorySize)
            {
                if (item.IsUnique)
                {
                    items.Add(item);
                    return true;
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].Id == item.Id && items[i].Amount < items[i].MaxStack)
                        {
                            items[i].Amount += item.Amount;
                            return true;
                        }
                    }
                    items.Add(item);
                    return true;
                }
            }
            return false;
        }

        public bool RemoveItem(int id, int amount)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    if (items[i].Amount > amount)
                    {
                        items[i].Amount -= amount;
                        return true;
                    }
                    else if (items[i].Amount == amount)
                    {
                        items.RemoveAt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckForItem(int id, int amount)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id && items[i].Amount >= amount)
                {
                    return true;
                }
            }
            return false;
        }

        public c_Item GetItem(int id)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    return items[i];
                }
            }
            return null;
        }

        public List<c_Item> GetItems(int id)
        {
            var foundItems = new List<c_Item>();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    foundItems.Add(items[i]);
                }
            }
            return foundItems;
        }


        public int GetSameItemsCount(int id)
        {
            int count = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Id == id)
                {
                    count++;
                }
            }
            return count;
        }
    
    
        
    
    }
}
