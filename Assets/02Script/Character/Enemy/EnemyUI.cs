using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    private Camera cam;
    private GameObject EnemyPointPrefab;
    private GameObject HPBarPrefab;
    private GameObject hpBar;
    private GameObject enemyPoint;
    private Vector3 hpOffset = new Vector3(0, 2f, 0);
    private Vector3 pointOffset = new Vector3(0, 6f, 0);
    private Image hp;
    private float maxHp;
    private float curHp;

    public void InitEnemyUI(float max)
    {
        cam = Camera.main;
        HPBarPrefab = Resources.Load<GameObject>("Prefabs/HPBar");
        EnemyPointPrefab = Resources.Load<GameObject>("Prefabs/EnemyPoint");

        hpBar = Instantiate(HPBarPrefab, transform.position + hpOffset, transform.root.rotation, transform);
        enemyPoint = Instantiate(EnemyPointPrefab, transform.position + pointOffset , Quaternion.identity, transform);

        if (!hpBar.transform.GetChild(1).TryGetComponent<Image>(out hp))
            Debug.Log("HPBar - Awake - Image");
        else
        {
            maxHp = max;
            curHp = max;
            hp.fillAmount = 1;
        }
        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        bool ready = false;
        if (!ready)
        {
            ready = true;
            yield return YieldInstructionCache.WaitForSeconds(1f);
        }
        while (true)
        {
            yield return null;
            hpBar.transform.rotation = transform.root.rotation;
        }
    }

    public void ChangeHP(float value)
    {
        curHp -= value;
        hp.fillAmount = curHp / maxHp;
    }

    public void ResetHP()
    {
        curHp = maxHp;
        hp.fillAmount = 1;
    }

    public void Die()
    {
        hpBar.SetActive(false);
        enemyPoint.SetActive(false);
    }

    public void Spawn()
    {
        hpBar.SetActive(true);
        hp.fillAmount = 1;
        curHp = maxHp;
        enemyPoint.SetActive(true);
    }
}
