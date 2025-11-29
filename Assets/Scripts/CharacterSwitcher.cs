using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    [Header("Animator Controllers")]
    public RuntimeAnimatorController lumenController;
    public RuntimeAnimatorController pyreController;
    public RuntimeAnimatorController rivenController;

    private Animator animator;
    private SpriteRenderer sprite;

    public enum Character
    {
        Lumen,
        Pyre,
        Riven
    }

    [Header("Current Character")]
    public Character currentCharacter = Character.Lumen;

    void Awake()
    {
        animator = GetComponent<Animator>();
        sprite   = GetComponent<SpriteRenderer>();   // SpriteRenderer'ı al
        ApplyCurrentController();
    }

    void Update()
    {
        // 1 = Lumen
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentCharacter = Character.Lumen;
            ApplyCurrentController();
        }
        // 2 = Pyre
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentCharacter = Character.Pyre;
            ApplyCurrentController();
        }
        // 3 = Riven
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentCharacter = Character.Riven;
            ApplyCurrentController();
        }
    }

    /// <summary>
    /// Seçili karaktere uygun Animator Controller ve yön (flip) ayarını uygular.
    /// </summary>
    private void ApplyCurrentController()
    {
        if (animator == null) return;

        switch (currentCharacter)
        {
            case Character.Lumen:
                animator.runtimeAnimatorController = lumenController;
                if (sprite != null)
                    sprite.flipX = false;   // Lumen sağ tarafa bakıyor
                break;

            case Character.Pyre:
                animator.runtimeAnimatorController = pyreController;
                if (sprite != null)
                    sprite.flipX = true;    // Pyre ters hareket ediyorsa TRUE yap
                break;

            case Character.Riven:
                animator.runtimeAnimatorController = rivenController;
                if (sprite != null)
                    sprite.flipX = false;   // Gerekirse bunu da TRUE yaparsın
                break;
        }
    }

    // İleride skill kodları için kısayol property'ler:
    public bool IsLumen => currentCharacter == Character.Lumen;
    public bool IsPyre  => currentCharacter == Character.Pyre;
    public bool IsRiven => currentCharacter == Character.Riven;
}
