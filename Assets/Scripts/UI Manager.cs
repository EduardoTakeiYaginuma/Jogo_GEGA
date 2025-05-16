using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerExperience playerExperience;
    [SerializeField] GameObject levelUpPanel;
    [SerializeField] HealthBar healthBar;
    [SerializeField] GameObject playerAura;
    [SerializeField] GameObject [] levelUpButtons;


    public void UpdateHealthBar(float currentHealth)
    {
        healthBar.SetHealth(currentHealth);
        hideLevelUpPanel();
    }


    public void IncreasePlayerSpeed(float speed)
    {
        playerMovement.walkSpeed += speed;
        playerMovement.runSpeed  += speed;
        hideLevelUpPanel();
    }

    public void IncreasePlayerAura(float radius)
    {
        playerAura.transform.localScale += playerAura.transform.localScale * radius;
        hideLevelUpPanel();
    }


    

    public void hideLevelUpPanel()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1; // Retorna o tempo ao normal
        playerMovement.enabled = true; // Habilita o movimento do jogador
        for (int i=0 ; i<3; i++){
            Destroy(levelUpPanel.transform.GetChild(i).gameObject);
        }
    }
    
    public void ShowLevelUpPanel()
    {
        
        for (int i=0; i<3; i++){
            Vector3 firstPosition = new Vector3(-600 + 600*i, 190, 0);
            int randomIndex = Random.Range(0, levelUpButtons.Length);

            GameObject spawned = Instantiate(levelUpButtons[randomIndex], firstPosition, levelUpButtons[randomIndex].transform.rotation, levelUpPanel.transform);
            RectTransform rectTransform = spawned.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = firstPosition;

            // Adiciona o evento de clique ao botão

            Button button = spawned.GetComponent<Button>();


            if (button != null)
            {
                button.onClick.AddListener(() => {
                    // Chama a função correspondente ao botão
                    if (randomIndex == 2)
                    {
                        IncreasePlayerSpeed(1.5f);
                    }
                    else if (randomIndex == 0)
                    {
                        UpdateHealthBar(100);
                    }
                    else if (randomIndex == 1)
                    {
                        IncreasePlayerAura(0.2f);
                    }
                });
            }

        }
        Time.timeScale = 0; // Pausa o jogo
        playerMovement.enabled = false; // Desabilita o movimento do jogador
        levelUpPanel.SetActive(true);
    }


}
