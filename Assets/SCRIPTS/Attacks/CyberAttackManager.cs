using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CyberAttackManager : MonoBehaviour
{
    [Header("Management System and Visualiser")]
    public PowerVisualiser powerVisualiser;
    public PowerSystemManager powerSystemManager;
    public PowerStatusIndicatorManager powerStatusIconManager;
    
    [Header("Attack Controls")]
    public Button visualDisruptionButton;
    public Button monitoringDisruptionButton;
    public Button powerGenerationButton;
    public Button substationAttackButton;

    [Header("Attack Lock Buttons")]
    public Button engageAttacksButton;
    public Button disengageAttacksButton;

    private Dictionary<string, CyberAttackBase> attacks = new Dictionary<string, CyberAttackBase>();

    private bool engageAttacks = false; // Lock control

    private void Awake()
    {
        CreateAttacks();
    }

    private void Start()
    {
        foreach (var attack in attacks.Values)
        {
            attack.Initialise(powerVisualiser, powerSystemManager);
        }

        SetupButtons();

        if (powerVisualiser != null)
        {
            powerVisualiser.cyberAttack = this;
        }

        UpdateLockButtons(); // Set initial button state
    }

    private void CreateAttacks()
    {
        VisualDisruptionAttack visualDisruptionAttack = gameObject.AddComponent<VisualDisruptionAttack>();
        visualDisruptionAttack.SetPowerStatusIndicator(powerStatusIconManager);
        attacks.Add(visualDisruptionAttack.AttackName, visualDisruptionAttack);

        MonitoringDisruptionAttack monitoringDisruptionAttack = gameObject.AddComponent<MonitoringDisruptionAttack>();
        monitoringDisruptionAttack.powerStatusIndicator = powerStatusIconManager;
        attacks.Add(monitoringDisruptionAttack.AttackName, monitoringDisruptionAttack);

        PowerGenerationAttack powerGenerationAttack = gameObject.AddComponent<PowerGenerationAttack>();
        attacks.Add(powerGenerationAttack.AttackName, powerGenerationAttack);

        SubstationAttack substationAttack = gameObject.AddComponent<SubstationAttack>();
        attacks.Add(substationAttack.AttackName, substationAttack);
    }

    private void SetupButtons()
    {
        if (visualDisruptionButton != null)
            visualDisruptionButton.onClick.AddListener(() => ToggleAttack("VisualDisruption", visualDisruptionButton));

        if (monitoringDisruptionButton != null)
            monitoringDisruptionButton.onClick.AddListener(() => ToggleAttack("MonitoringDisruption", monitoringDisruptionButton));

        if (powerGenerationButton != null)
            powerGenerationButton.onClick.AddListener(() => ToggleAttack("PowerGeneration", powerGenerationButton));

        if (substationAttackButton != null)
            substationAttackButton.onClick.AddListener(() => ToggleAttack("SubstationAttack", substationAttackButton));

        if (engageAttacksButton != null)
            engageAttacksButton.onClick.AddListener(() => SetEngageAttacks(true));

        if (disengageAttacksButton != null)
            disengageAttacksButton.onClick.AddListener(() => SetEngageAttacks(false));
    }

    public void ToggleAttack(string attackName, Button associatedButton = null)
    {
        if (!engageAttacks)
        {
            Debug.LogWarning("Attacks are currently locked. Click 'Engage Attacks' to enable them.");
            return;
        }

        if (attacks.TryGetValue(attackName, out CyberAttackBase attack))
        {
            attack.ToggleAttack();

            if (associatedButton != null)
            {
                UpdateButtonState(associatedButton, attack.IsActive(), attackName);
            }
        }
        else
        {
            Debug.LogError($"Attack '{attackName}' not found!");
        }
    }

    public bool IsTurbineManipulated(Turbine turbine)
    {
        if (attacks.TryGetValue("PowerGeneration", out CyberAttackBase attack) &&
            attack is PowerGenerationAttack powerGenAttack &&
            powerGenAttack.IsActive())
        {
            return powerGenAttack.IsTurbineManipulated(turbine);
        }

        return false;
    }

    private void UpdateButtonState(Button button, bool active, string attackName)
    {
        if (button == null) return;

        TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();
        Text regularText = button.GetComponentInChildren<Text>();

        ColorBlock colors = button.colors;
        colors.normalColor = active ? Color.red : Color.white;
        button.colors = colors;
    }

    private void SetEngageAttacks(bool value)
    {
        engageAttacks = value;
        Debug.Log("Cyber Attacks " + (engageAttacks ? "Enabled" : "Disabled"));
        UpdateLockButtons();

        // Optionally, you could deactivate all attacks when disengaging
        if (!engageAttacks)
        {
            foreach (var attack in attacks.Values)
            {
                if (attack.IsActive())
                    attack.ToggleAttack(); // turn off active attacks
            }

            // Reset button visuals
            UpdateButtonState(visualDisruptionButton, false, "");
            UpdateButtonState(monitoringDisruptionButton, false, "");
            UpdateButtonState(powerGenerationButton, false, "");
            UpdateButtonState(substationAttackButton, false, "");
        }
    }

    private void UpdateLockButtons()
    {
        if (engageAttacksButton != null)
            engageAttacksButton.gameObject.SetActive(!engageAttacks);

        if (disengageAttacksButton != null)
            disengageAttacksButton.gameObject.SetActive(engageAttacks);
    }

    private void OnDestroy()
    {
        if (visualDisruptionButton != null) visualDisruptionButton.onClick.RemoveAllListeners();
        if (monitoringDisruptionButton != null) monitoringDisruptionButton.onClick.RemoveAllListeners();
        if (powerGenerationButton != null) powerGenerationButton.onClick.RemoveAllListeners();
        if (substationAttackButton != null) substationAttackButton.onClick.RemoveAllListeners();
        if (engageAttacksButton != null) engageAttacksButton.onClick.RemoveAllListeners();
        if (disengageAttacksButton != null) disengageAttacksButton.onClick.RemoveAllListeners();
    }
}
