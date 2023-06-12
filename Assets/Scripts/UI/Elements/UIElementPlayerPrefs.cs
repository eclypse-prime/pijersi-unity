using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Selectable))]
public class UIElementPlayerPrefs : MonoBehaviour
{
    private delegate void SimpleDelegate();
    private SimpleDelegate set;
    private SimpleDelegate get;

    private delegate bool BoolDelegate();
    private BoolDelegate isEqual;

    private Slider slider;
    private Dropdown dropdown;
    private Toggle toggle;
    private TMP_Dropdown TmpDropdown;

    private void Awake() => Init();
    private void Start() => GetPref();
    private void OnDestroy() => SetPref();

    private void Init()
    {
        if (TryInitWith(out slider, SliderToPref, PrefToSlider, IsSliderEqualPref))
            return;
        if (TryInitWith(out dropdown, DropdownToPref, PrefToDropdown, IsDropdownEqualPref))
            return;
        if (TryInitWith(out toggle, ToggleToPref, PrefToToggle, IsToggleEqualPref))
            return;
        if (TryInitWith(out TmpDropdown, TmpDropdownToPref, PrefToTmpDropdown, IsTmpDropdownEqualPref))
            return;

        Debug.LogError($"Can find valid selectable on '{name}'.");
    }

    private bool TryInitWith<T>(out T component, SimpleDelegate set, SimpleDelegate get, BoolDelegate isEqual) where T : Component
    {
        if (!TryGetComponent(out component)) return false;

        this.set = set;
        this.get = get;
        this.isEqual = isEqual;

        return true;
    }

    private void GetPref()
    {
        if (!PlayerPrefs.HasKey(name))
        {
            set.Invoke();
            return;
        }

        get.Invoke();
    }

    private void SetPref()
    {
        if (isEqual.Invoke()) return;

        set.Invoke();
    }

    // Pref setters
    private void SliderToPref() => PlayerPrefs.SetFloat(name, slider.value);
    private void DropdownToPref() => PlayerPrefs.SetInt(name, dropdown.value);
    private void ToggleToPref() => PlayerPrefs.SetInt(name, toggle.isOn ? 1 : 0);
    private void TmpDropdownToPref() => PlayerPrefs.SetInt(name, TmpDropdown.value);

    // Pref getters
    private void PrefToSlider() => slider.value = PlayerPrefs.GetFloat(name);
    private void PrefToDropdown() => dropdown.value = PlayerPrefs.GetInt(name);
    private void PrefToToggle() => toggle.isOn = PlayerPrefs.GetInt(name) == 1;
    private void PrefToTmpDropdown() => TmpDropdown.value = PlayerPrefs.GetInt(name);

    // isEqual
    private bool IsSliderEqualPref() => PlayerPrefs.GetFloat(name) == slider.value;
    private bool IsDropdownEqualPref() => PlayerPrefs.GetInt(name) == dropdown.value;
    private bool IsToggleEqualPref() => PlayerPrefs.GetInt(name) == (toggle.isOn ? 1 : 0);
    private bool IsTmpDropdownEqualPref() => PlayerPrefs.GetInt(name) == TmpDropdown.value;
}