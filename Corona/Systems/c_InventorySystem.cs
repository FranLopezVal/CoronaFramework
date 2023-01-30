using Corona.Systems;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

using UnityEngine;

namespace Corona
{
    /// <summary>
    /// Sistema de Inventario propio de Corona.
    /// Con guardado de inventarios.
    /// </summary>
    internal class c_InventorySystem : c_System
    {
        bool _use_corona_inventory_system = true;
        
        string _path_save_inv = "";
        string _path_icons="";

        public List<Inventory>? globals_inventories;

        int _last_inventory_id;
        
        /// <summary>
        /// Devuelve el ultimo indice de Inventarios.
        /// </summary>
        /// <returns></returns>
        public int GetLastIndex()
        {
            return _last_inventory_id++;
        }

        /// <summary>
        /// Inicia el sistema de Inventario de Corona, usara datos preconfiguados de Corona.
        /// </summary>
        /// <param name="e"></param>
        public override void Init(c_SystemEventArg? e)
        {
            if(globals_inventories==null)
                globals_inventories = new List<Inventory>();
            
            _use_corona_inventory_system = Corona.Get<bool>("Corona::InventorySystem");
            _path_save_inv = Corona.Get(CoronaOptions.saveGame);
            _path_icons = Corona.Get(CoronaOptions.iconsPath);
            base.Init(e);
        }

        /// <summary>
        /// Actualiza el sistema de corona.
        /// </summary>
        /// <param name="e"></param>
        public override void Update(c_SystemEventArg? e)
        {
            base.Update(e);
        }
     
        /// <summary>
        /// Devuelve si se usa el sistema de inventario de Corona.
        /// </summary>
        /// <returns></returns>
        public override bool IsActive()
        {
            return _use_corona_inventory_system;
        }

        /// <summary>
        /// guarda el inventario poniendo de referencia el ID.
        /// </summary>
        /// <param name="inv"></param>
        /// <returns></returns>
        private bool SaveInventory(Inventory inv)
        {
            if(!IsActive())return false;

            string path = Path.Combine(_path_save_inv, "dt_" + inv.GetId() + "_i.dat");

            List<SaveInventoryData> sid = new List<SaveInventoryData>();

            foreach (var item in inv.AllItems)
            {
                sid.Add(new SaveInventoryData
                {
                    name_ref = item.Name,
                    amount = item.Amount,
                    description_text_ref = item.Description_text_ref,
                    icon_path = item.Icon.texture.name + "|" + item.Icon.name,
                    id = item.Id, isUnique = item.IsUnique, isUsable = item.IsUsable,
                    maxStack = item.MaxStack
                });
            }

            XmlSerializer serializer = new XmlSerializer(typeof(SaveInventoryData[]));
            using (FileStream stream = new FileStream("item.xml", FileMode.Create))
            {
                serializer.Serialize(stream,sid);
            }


            return true;
        }
       
        /// <summary>
        /// Carga los datos desde un archivo de guardado, la referencia de ese 
        /// archivo es el ID de el inventario.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Inventory LoadInventory(int id)
        {
            if (!IsActive()) return null;

            string path = Path.Combine(_path_save_inv,"dt_" + id + "_i.dat");
            Inventory inv = new Inventory();

            XmlSerializer deserializer = new XmlSerializer(typeof(SaveInventoryData[]));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                SaveInventoryData[] d_item = (SaveInventoryData[])deserializer.Deserialize(stream);
                foreach (var item in d_item)
                {
                    Sprite sp = Resources.Load<Sprite>(Path.Combine(_path_icons, item.icon_path.Split('|')[1]));
                    inv.AddItem(new c_Item
                    {
                        Name = item.name_ref,
                        Description_text_ref = item.description_text_ref,Icon = sp,
                        Amount = item.amount,Id = item.id,
                        IsUnique = item.isUnique, IsUsable = item.isUsable,
                        MaxStack = item.maxStack
                    });
                }
            }
            return inv;
        }

    }

    /// <summary>
    /// Clase de apoyo para guardar los items de un inventario.
    /// </summary>
    [XmlRoot(elementName:"Items")]
    public struct SaveInventoryData
    {
        public string name_ref;
        public int id;
        public string description_text_ref;
        public string icon_path;
        public int amount;
        public int maxStack;
        public bool isUnique;
        public bool isUsable;
    }
}
