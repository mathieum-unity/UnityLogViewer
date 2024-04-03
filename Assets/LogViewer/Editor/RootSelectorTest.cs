using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RootSelectorTest : EditorWindow
{
    [SerializeField] private StyleSheet m_StyleSheet = default;

    [MenuItem("Window/RootSelectorTest")]
    public static void ShowExample()
    {
        RootSelectorTest wnd = GetWindow<RootSelectorTest>();
        wnd.titleContent = new GUIContent("RootSelectorTest");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        VisualElement myElement = new VisualElement() { name = "my-element" };

        Label myLabel = new Label() { name = "my-label" };
        myLabel.text = "my label";

        Label mySecondLabel = new Label() { name = "my-second-label" };
        mySecondLabel.text = "2nd label";

        var myOtherElement = new VisualElement() { name = "my-other-element" };
        var myOtherElementChild = new VisualElement() { name = "my-other-element-child" };

        myElement.Add(myLabel);
        myLabel.Add(mySecondLabel);
        myElement.Add(myOtherElement);
        myOtherElement.Add(myOtherElementChild);
        root.Add(myElement);

        myElement.styleSheets.Add(m_StyleSheet);


        var button = new Button();
        button.text = "Toggle style on label";

        button.clicked += () =>
        {
            if (myLabel.styleSheets.Contains(m_StyleSheet))
            {
                myLabel.styleSheets.Remove(m_StyleSheet);
            }
            else
            {
                myLabel.styleSheets.Add(m_StyleSheet);
            }
        };
        root.Add(button);

        button = new Button();
        button.text = "Print Flex direction on all elements";

        button.clicked += () => { Debug.Log(PrintFlexDirection(myElement)); };
        root.Add(button);
    }

    static string PrintFlexDirection(VisualElement element, int depth = 0)
    {
        string result = "";

        for (int i = 0; i < depth; ++i)
        {
            result += "  ";
        }

        result = $"Element \"{element.name}\" Flex-direction:" + element.resolvedStyle.flexDirection + "\n";
        for (int i = 0; i < element.childCount; ++i)
        {
            result += PrintFlexDirection(element[i], depth + 1);
        }

        return result;
    }
}