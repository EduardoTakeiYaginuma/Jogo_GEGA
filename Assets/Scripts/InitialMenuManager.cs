using UnityEngine;
using UnityEngine.SceneManagement;

public class InitMenuManager : MonoBehaviour
{
    public void IniciaJogo()
    {
        SceneManager.LoadScene(1);
    }
    
    public void GoInitial()
    {
        SceneManager.LoadScene(0);
    }
   
    public void GoInstrucoes()
    {
        SceneManager.LoadScene(2);
    }

}