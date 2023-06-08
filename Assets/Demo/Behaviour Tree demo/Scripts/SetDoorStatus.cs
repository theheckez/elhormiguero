using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetDoorStatus : MonoBehaviour {

    #region variables

    [SerializeField] private Transform keySpawn;
    [SerializeField] private GameObject key;

    private GameObject testBoy;
    private TestBoyBT testBoyBT;
    private Slider doorStatusSlider;

    #endregion variables

    private void Start()
    {
        doorStatusSlider = GetComponent<Slider>();
    }

    public void SetDoorState()
    {
        if(testBoy == null) {
            testBoy = GameObject.FindGameObjectWithTag("Player");
            testBoyBT = testBoy.GetComponent<TestBoyBT>();
        }

        switch(doorStatusSlider.value) {
            case 0:
                testBoyBT.DoorState = DoorStatus.Opened;
                DestroyKey();
                break;

            case 1:
                testBoyBT.DoorState = DoorStatus.Locked;
                CreateKey();
                break;

            case 2:
                testBoyBT.DoorState = DoorStatus.Closed;
                DestroyKey();
                break;
        }
    }

    private void DestroyKey()
    {
        Destroy(GameObject.FindGameObjectWithTag("Key"));
    }

    private void CreateKey()
    {
        Instantiate(key, keySpawn.position, keySpawn.rotation);
    }
}