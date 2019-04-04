using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : MyTools, IPlayerPart {

    [Header("Точка, где будет расположено оружие")]
    [Tooltip("При спавне оружие будет появляться в этой точке. Требуется заранее подгонять прифабы, чтобы смещение префаба передавалось" +
        "и появляющемуся экземпляру оружия. Тем самым любой префа будет располагаться в руке правильно.")]
    public Transform weaponPoint;
    public Transform fightPoint;

    [Space(10)]
    public Text itemText;

    [HideInInspector]
    public Camera sniperCam;

    private CharacterInput characterInput;
    private CharacterIK characterIK;
    private CharacterFight characterFight;
    private CharacterStatus characterStatus;

    private GameObject cameraSystem;
    private GameObject cam;
    private GameObject objWeapon;
    private GameObject sword;

    [Tooltip("Основное")] public WeaponConfig firstWeapon;
    [Tooltip("Писторлет")] public WeaponConfig secondWeapon;
    [Tooltip("Ближний бой")] public WeaponConfig fightWeapon;

    private Animator anim;
    private Transform targetLook;
    private ShootWeapon activeShootWeapon;
    private FightWeapon activeFightWeapon;
    private LayerMask noPlayerMask;
    private Quaternion rotRight;
    private Slider ammoSlider;

    [HideInInspector] public bool fighter;

    public void InventoryUpdate()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 15f,noPlayerMask))
        {
            if (hit.collider.tag.Equals("Item"))
            {
                Item item = hit.collider.gameObject.GetComponent<Item>();
                itemText.text = item.itemName + " \r\n [E] - подобрать.";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    itemText.text = string.Empty;
                    if (item.firstWeapon)
                    {
                        if (firstWeapon != null)
                        {
                            Instantiate(firstWeapon.itemPrefab, hit.collider.transform.position, hit.collider.transform.rotation);
                        }
                        DestroyWeapon();
                        firstWeapon = item.weaponConfig;
                        characterInput.selectedWeapon = 1;
                        anim.SetTrigger("ChangeWeapon");
                        Destroy(hit.collider.gameObject);
                    }
                    else
                    {
                        if (secondWeapon != null)
                        {
                            Instantiate(secondWeapon.itemPrefab, hit.collider.transform.position, hit.collider.transform.rotation);
                        }
                        DestroyWeapon();
                        secondWeapon = item.weaponConfig;
                        characterInput.selectedWeapon = 2;
                        anim.SetTrigger("ChangeWeapon");
                        Destroy(hit.collider.gameObject);
                    }
                }
            }
            else
            {
                itemText.text = string.Empty;
            }
        }
        else
        {
            itemText.text = string.Empty;
        }
    }
    public void SelectWeaponAction(int selectedWeapon)
    {
        if (selectedWeapon == 1 && firstWeapon != null)
        {
            objWeapon = Instantiate(firstWeapon.weaponPrefab, weaponPoint);
            objWeapon.transform.localPosition = firstWeapon.weaponPos;
            objWeapon.transform.localRotation = Quaternion.Euler(firstWeapon.weaponRot.x,
                firstWeapon.weaponRot.y, firstWeapon.weaponRot.z);
            activeShootWeapon = objWeapon.GetComponent<ShootWeapon>();
            activeShootWeapon.ammoSlider = ammoSlider;
            ammoSlider.value = 0;

            if(activeShootWeapon.sniperCam != null)
            {
                characterStatus.sniper = true;
                sniperCam = activeShootWeapon.sniperCam;
            }

            characterIK.rHand.localPosition = firstWeapon.rHandPos;
            rotRight = Quaternion.Euler(firstWeapon.rHandRot.x, firstWeapon.rHandRot.y, firstWeapon.rHandRot.z);
            characterIK.rHand.localRotation = rotRight;

            activeShootWeapon.targetLook = targetLook;
            activeShootWeapon.cameraMain = cameraSystem;
            characterInput.shootWeapon = activeShootWeapon;
            characterIK.lHandTarget = activeShootWeapon.leftHandTarget;
            characterInput.SetShootDelegate(firstWeapon.type);

            anim.SetBool("Weapon", true);
            anim.SetInteger("WeaponType", 2);
        }
        if (selectedWeapon == 2 && secondWeapon != null)
        {
            objWeapon = Instantiate(secondWeapon.weaponPrefab, weaponPoint);
            objWeapon.transform.localPosition = secondWeapon.weaponPos;
            objWeapon.transform.localRotation = Quaternion.Euler(secondWeapon.weaponRot.x,
                secondWeapon.weaponRot.y, secondWeapon.weaponRot.z);
            activeShootWeapon = objWeapon.GetComponent<ShootWeapon>();
            activeShootWeapon.ammoSlider = ammoSlider;
            ammoSlider.value = 0;

            characterIK.rHand.localPosition = secondWeapon.rHandPos;
            rotRight = Quaternion.Euler(secondWeapon.rHandRot.x, secondWeapon.rHandRot.y, secondWeapon.rHandRot.z);
            characterIK.rHand.localRotation = rotRight;

            activeShootWeapon.targetLook = targetLook;
            activeShootWeapon.cameraMain = cameraSystem;
            characterInput.shootWeapon = activeShootWeapon;
            characterIK.lHandTarget = activeShootWeapon.leftHandTarget;
            characterInput.SetShootDelegate(secondWeapon.type);

            anim.SetBool("Weapon", true);
            anim.SetInteger("WeaponType", 1);
        }
        if (selectedWeapon == 3)
        {
            anim.SetBool("Weapon", true);
            anim.SetInteger("WeaponType", 0);
        }
    }

    public void DestroyWeapon()
    {
        characterStatus.sniper = false;
        characterInput.shootWeapon = null;
        characterIK.lHandTarget = null;
        if(objWeapon != null)
        {
            Destroy(objWeapon);
        }
        objWeapon = null;
        anim.SetBool("Weapon", false);
        anim.SetInteger("WeaponType", 0);
    }
    public void RemoveAllShootWeapon()
    {
        firstWeapon = null;
        secondWeapon = null;
    }

    public void Initiolize(SampleController sampleController)
    {
        characterStatus = sampleController.characterStatus;
        targetLook = sampleController.cameraHandler.targetLook;
        cameraSystem = sampleController.cameraHandler.mainTransform.gameObject;
        noPlayerMask = sampleController.cameraHandler.noPlayerMask;
        cam = sampleController.cameraHandler.cameraTransform.gameObject;
        anim = sampleController.anim;
        characterInput = sampleController.characterInput;
        characterFight = sampleController.characterFight;
        characterIK = sampleController.characterIK;
        ammoSlider = sampleController.ammoSlider;

        if(characterFight != null)
        {
            sword = Instantiate(fightWeapon.weaponPrefab, fightPoint);
            ReturnSwordToPos();
            activeFightWeapon = sword.GetComponent<FightWeapon>();
            characterInput.fightWeapon = activeFightWeapon;
            characterFight.weapon = activeFightWeapon;

            anim.SetBool("Weapon", true);
            anim.SetInteger("WeaponType", 0);
            fighter = true;
        }
        itemText.text = string.Empty;
    }

    public void ReturnSwordToPos()
    {
        sword.transform.localPosition = fightWeapon.weaponPos;
        sword.transform.localRotation = Quaternion.Euler(fightWeapon.weaponRot.x,
            fightWeapon.weaponRot.y, fightWeapon.weaponRot.z);
    }
}
