using UnityEngine;

public static class AutoSceneSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        Debug.Log("AutoSceneSetup: Initializing combat scene elements...");

        SetupDamageEffect();
        SetupMonster();
        SetupPlayer();
    }

    private static void SetupMonster()
    {
        if (GameObject.Find("Monster") == null)
        {
            GameObject monster = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            monster.name = "Monster";
            monster.transform.position = new Vector3(5, 1, 5);
            
            // Add components
            if (monster.GetComponent<Health>() == null) monster.AddComponent<Health>();
            if (monster.GetComponent<MonsterController>() == null) monster.AddComponent<MonsterController>();
            
            // Set Color to differentiate
            Renderer rend = monster.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = Color.green; // Monster is Green
            }

            Debug.Log("AutoSceneSetup: Created Monster.");
        }
    }

    private static void SetupDamageEffect()
    {
        if (GameObject.Find("DamageEffectPrototype") == null)
        {
            GameObject effect = GameObject.CreatePrimitive(PrimitiveType.Cube);
            effect.name = "DamageEffectPrototype";
            effect.transform.position = new Vector3(0, -100, 0); // Hide it
            effect.transform.localScale = Vector3.one * 0.3f; // Smaller
            
            if (effect.GetComponent<DamageEffect>() == null) effect.AddComponent<DamageEffect>();
            
            // Remove collider so it doesn't interfere
            Collider col = effect.GetComponent<Collider>();
            if (col != null)
            {
                Object.Destroy(col);
            }
            
            Debug.Log("AutoSceneSetup: Created DamageEffectPrototype.");
        }
    }

    private static void SetupPlayer()
    {
        // Try find by tag first, then name
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("Player");
        if (player == null) player = GameObject.Find("PlayerArmature"); // Common name in starter assets

        if (player != null)
        {
            if (player.GetComponent<Health>() == null) 
                player.AddComponent<Health>();
                
            if (player.GetComponent<PlayerCombat>() == null) 
                player.AddComponent<PlayerCombat>();
                
            Debug.Log($"AutoSceneSetup: Configured Player ({player.name}).");
        }
        else
        {
            Debug.LogWarning("AutoSceneSetup: Player not found!");
        }
    }
}
