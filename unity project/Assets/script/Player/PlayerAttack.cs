using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{   
    [Header("Config")]
    [SerializeField] private Weapon initialWeapon;
    [SerializeField] private Transform[] attackPositions;

    private PlayerActions actions;
    private PlayerAnimations playerAnimations;

    private PlayerMovement playerMovement;

    private PlayerMana playerMana;
    private EnemyBrain enemyTarget;
    private Coroutine attackCoroutine;

    private Transform currentAttackPosition;
    private float currentAttackRotation;

    private void Awake()
    {
        actions = new PlayerActions();
        playerMana = GetComponent<PlayerMana>();
        playerMovement =GetComponent<PlayerMovement>();
        playerAnimations = GetComponent<PlayerAnimations>();
    } 

    private void Start()
    {
        actions.Attack.ClickAttack.performed += ctx => Attack();
    }

    private void Update()
    {
        GetFirePosition();
    }
    private void Attack()
    {
        if (enemyTarget == null) return;
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(IEAttack());
    }

    private IEnumerator IEAttack()
    {   
        if (currentAttackPosition != null)
        {   
            if (playerMana.CurrentMana < initialWeapon.RequiredMana) yield break;
            Quaternion rotation = 
                Quaternion.Euler(new Vector3(0f, 0f, currentAttackRotation));
            Projectile projectile = Instantiate(initialWeapon.ProjectilePrefab, 
                currentAttackPosition.position, rotation);
            projectile.Direction = Vector3.up;
            projectile.Damage = initialWeapon.Damage;;
            playerMana.UseMana(initialWeapon.RequiredMana);
        }

        playerAnimations.SetAttackAnimation(true);
        yield return new WaitForSeconds(0.5f);
        playerAnimations.SetAttackAnimation(false);
    }


    private void GetFirePosition()
    {
        Vector2 moveDirection = playerMovement.MoveDirection;
        switch (moveDirection.x)
        {
            case > 0f:
                currentAttackPosition = attackPositions[1];
                currentAttackRotation = -90f;
                break; 
            case < 0f:
                currentAttackPosition = attackPositions[3];
                currentAttackRotation = -270f;
                break;
        }
        switch (moveDirection.y)
        {
            case > 0f:
                currentAttackPosition = attackPositions[0];
                currentAttackRotation = -90f;
                break; 
            case < 0f:
                currentAttackPosition = attackPositions[2];
                currentAttackRotation = -270f;
                break;
            
        }

    }
    private void EnemySelectedCallback(EnemyBrain enemySelected)
    {
        enemyTarget = enemySelected;
    }

    private void NoEnemySelectionCallback()
    {
        enemyTarget = null;
    }

    private void OnEnable()
    {
        actions.Enable();
        SelectionManager.OnEnemySelectedEvent += EnemySelectedCallback;
        SelectionManager.OnNoSelectionEvent += NoEnemySelectionCallback;
    }

    private void OnDisable()
        {
            actions.Disable();
             SelectionManager.OnEnemySelectedEvent -= EnemySelectedCallback;
             SelectionManager.OnNoSelectionEvent -= NoEnemySelectionCallback;
        }
    
}
