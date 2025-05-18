using UnityEngine;
using UnityEngine.SceneManagement;

public class InitMenuManager : MonoBehaviour
{
    public void IniciaJogo()
    {
        SceneManager.LoadScene(2);
    }
    
    public void GoInitial()
    {
        SceneManager.LoadScene(1);
    }
   
    public void GoInstrucoes()
    {
        SceneManager.LoadScene(3);
    }

}