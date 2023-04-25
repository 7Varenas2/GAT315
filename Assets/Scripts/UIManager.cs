using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.General
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] Slider healthMeter;
        [SerializeField] TMP_Text scoreUI;
        [SerializeField] GameObject gameOverUI;
        [SerializeField] GameObject gameWonUI;
        [SerializeField] GameObject titleUI;

        public void ShowTitle(bool show)
        {
            titleUI.SetActive(show);
        }

        public void ShowGameOver(bool show)
        {
            gameOverUI.SetActive(show);
        }

        public void ShowGameWin(bool show)
        {
            gameWonUI.SetActive(show);
        }

        public void SetHealth(int health)
        {
            healthMeter.value = Mathf.Clamp(health, 0, 100);
        }

        public void SetScore(int score)
        {
            scoreUI.text = score.ToString();
        }

    }
}
