using UnityEngine;
using TMPro;
using NUnit.Framework;
using Unity.VisualScripting;

public class MenuSelectorController : MonoBehaviour
{
    public TextMeshProUGUI label;
    public string level;
    public string className;
    public EnemySpawner spawner;
    //public PlayerController player;
    static string SelectedLevel;
    static string SelectedClass;
    static EnemySpawner SelectedEnemySpawner;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(string text)
    {
        level = text;
        label.text = text;
    }

    public void SetClass(string text)
    {
        //Debug.Log("setting class to " + text);
        className = text;
        label.text = text;
    }

    public void SelectClass()
    {
        
    }

    public void SelectLevel()
    {
    
    }

    public void StartLevel()
    {
        switch (GameManager.Instance.state)
        {
            case GameManager.GameState.PREGAME:

                if (spawner != null) // if this is a class button
                {
                    SelectedLevel = level;
                    SelectedEnemySpawner = spawner;
                }
                else if(label.text != "Start")// if this is a level button
                {
                    SelectedClass = className;
                    //Debug.Log("Selecting class: " + className);
                }

                if (label.text == "Start" && SelectedClass != null && SelectedLevel != null) // if it's the start button and the other parameters are filled
                {
                    //Debug.Log("Selected Level = " + SelectedLevel);
                    //Debug.Log("Selected Class = " + SelectedClass);
                    SelectedEnemySpawner.StartLevel(SelectedLevel, SelectedClass);

                }

                
            break;
            case GameManager.GameState.WAVEEND:
                SelectedEnemySpawner.NextWave();
            break;
            case GameManager.GameState.GAMEOVER:
                SelectedEnemySpawner.RestartGame();
            break;
        }   

    }

    
}
