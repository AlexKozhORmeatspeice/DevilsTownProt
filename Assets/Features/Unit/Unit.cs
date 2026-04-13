using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int maxNumFavouriteDisks = 2;
    [SerializeField] private GameObject deadBody;

    private List<ItemData> favouriteDisks = new();

    void Start()
    {
        if(ItemSystem.instance.TryGetGroupByID(ItemGroupAPI.Disk, out var diskGroup))
        {
            List<ItemData> favDisk = diskGroup.items.OrderBy(x => Random.value).Take(maxNumFavouriteDisks).ToList();

            favouriteDisks.AddRange(favDisk);
        }
    }

    public bool isFavouriteDisk(ItemData item)
    {
        return favouriteDisks.Contains(item);
    }

    public void Kill()
    {
        InvestSystem.instance.AddKillUnit();

        Instantiate(deadBody, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
