using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Linq;

//To get around lack of MI (I think, feel free to change if stupid)
public interface IStorage
{
    Storage Store
    {
        get;
    }
}

public class Storage
{
    private class ResourceStorage
    {
        public int NumberInStore;
        public e_Resource StoredResource;

        public ResourceStorage(e_Resource ToStore)
        {
            StoredResource = ToStore;
            NumberInStore = 0;
        }
    }

    private int m_MaxStackSize = 5;
    private int m_MaxAmountOfSlotsAvalible = 5;

    public int MaxStackSize
    {
        get
        {
            return m_MaxStackSize;
        }
    }

    public int MaxAmountOfStorageSlots
    {
        get
        {
            return m_MaxAmountOfSlotsAvalible;
        }
    }

    public Storage(int StackSize, int Slots)
    {
        m_MaxStackSize = StackSize;
        m_MaxAmountOfSlotsAvalible = Slots;
    }

    List<ResourceStorage> m_Storage = new List<ResourceStorage>();

    public bool CheckResource(e_Resource Resource, int AmountToCheckFor)
    {
        return m_Storage.Exists(S => S.StoredResource == Resource && S.NumberInStore >= AmountToCheckFor);
    }

    public bool TakeResource(e_Resource Resource, int AmountToTake)
    {
        //I can't spell
        bool TakeSucceded = false;
        ResourceStorage Slot = m_Storage.FirstOrDefault(S => S.StoredResource == Resource);

        if(Slot != null && Slot.NumberInStore >= AmountToTake)
        {
            Slot.NumberInStore -= AmountToTake;

            if(Slot.NumberInStore == 0)
            {
                m_Storage.Remove(Slot);
            }

            TakeSucceded = true;
        }

        return TakeSucceded;
    }

    public bool CanStoreResource(e_Resource Resource, int ExtraStackSpaceNeeded)
    {
        bool CanStore = false;
        ResourceStorage Slot = m_Storage.FirstOrDefault(S => S.StoredResource == Resource);

        if(Slot != null)
        {
            if(Slot.NumberInStore + ExtraStackSpaceNeeded <= this.MaxStackSize)
            {
                CanStore = true;
            }
        }
        else
        {
            if(m_Storage.Count < this.MaxAmountOfStorageSlots && ExtraStackSpaceNeeded <= this.MaxStackSize)
            {
                CanStore = true;
            }
        }

        return CanStore;
    }

    public bool StoreResource(e_Resource Resource, int AmountToStore)
    {
        bool CanStore = CanStoreResource(Resource, AmountToStore);

        if(CanStore)
        {
            ResourceStorage Slot = FindOrCreateSlot(Resource);
            Slot.NumberInStore += AmountToStore;
        }

        return CanStore;
    }

    public bool TransferFrom(Storage Store, e_Resource Resource, int AmountToTake)
    {
        bool CanTake    = Store.CheckResource(Resource, AmountToTake);
        bool CanStore   = CanStoreResource(Resource, AmountToTake);

        if(CanTake && CanStore)
        {
            Store.TakeResource(Resource, AmountToTake);
            StoreResource(Resource, AmountToTake);
        }

        return CanTake && CanStore;
    }

    public bool TransferTo(Storage Store, e_Resource Resource, int AmountToStore)
    {
        bool CanStore    = Store.CanStoreResource(Resource, AmountToStore);
        bool CanTake     = CheckResource(Resource, AmountToStore);

        if(CanTake && CanStore)
        {
            TakeResource(Resource, AmountToStore);
            Store.StoreResource(Resource, AmountToStore);
        }

        return CanTake && CanStore;
    }

    ResourceStorage FindOrCreateSlot(e_Resource Resource)
    {
        ResourceStorage Slot = m_Storage.FirstOrDefault(S => S.StoredResource == Resource);

        if(Slot == null)
        {
            Slot = new ResourceStorage(Resource);
            m_Storage.Add(Slot);
        }

        return Slot;
    }
}
