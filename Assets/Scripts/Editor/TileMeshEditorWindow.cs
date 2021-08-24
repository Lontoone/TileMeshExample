using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

public class TileMeshEditorWindow : EditorWindow
{
    private TileMeshEditor editor;
    private ObjectField imageCollectionField;
    private TileImageCollection imageCollection;

    private bool isDrawing = false;

    private Button drawButton;

    //Test:
    private Tile _previousTile, _currentSelectedTile;
    public int paintLayer = 1;


    [OnOpenAsset(1)]
    public static bool ShowWindowInfo(int _instanceID, int line)
    {
        UnityEngine.Object item = EditorUtility.InstanceIDToObject(_instanceID);
        if (item is TileMeshData)
        {
            TileMeshEditorWindow window = (TileMeshEditorWindow)GetWindow(typeof(TileMeshEditorWindow));
            window.titleContent = new GUIContent("Tile Mesh Drawer");
            //window.current_dialogContainer = item as DialogContainerSO;
            window.minSize = new Vector2(150, 150);
            //window.Load();

        }
        return false;
    }
    private void OnEnable()
    {
        isDrawing = false;
        SceneView.duringSceneGui += OnScene;
        CreateTooBar();
        //Graph Preview
    }

    private void OnScene(SceneView scene)
    {
        if (isDrawing)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        if (isDrawing &&
            (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) &&
            Event.current.button == 0)
        {
            DrawOnScene(scene);
        }
    }

    private void CreateTooBar()
    {
        Toolbar toolbar = new Toolbar();

        Button saveBtn = new Button()
        {
            text = "Save"
        };
        saveBtn.clicked += Save;

        Button loadBtn = new Button()
        {
            text = "Load"
        };
        loadBtn.clicked += Load;

        imageCollectionField = new ObjectField
        {
            objectType = typeof(TileImageCollection),
            allowSceneObjects = false,
            value = imageCollection
        };
        imageCollectionField.RegisterValueChangedCallback(value =>
        {
            imageCollection = value.newValue as TileImageCollection;
        });

        toolbar.Add(saveBtn);
        toolbar.Add(loadBtn);
        rootVisualElement.Add(toolbar);


        rootVisualElement.Add(imageCollectionField);
        drawButton = new Button()
        {
            text = "Draw"
        };
        drawButton.clicked += StartDraw;
        rootVisualElement.Add(drawButton);
    }


    private void Save() { }
    private void Load() { }
    private void StartDraw()
    {
        isDrawing = !isDrawing;

        if (isDrawing)
        {
            drawButton.text = "Stop";
        }
        else
        {
            drawButton.text = "Draw";
        }
    }


    private void DrawOnScene(SceneView scene)
    {
        if (imageCollection == null) { return; }

        // 当前屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
        Vector2 mousePosition = Event.current.mousePosition;
        // Retina 屏幕需要拉伸值
        float mult = 1;
        mult = EditorGUIUtility.pixelsPerPoint;
        // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
        mousePosition.y = scene.camera.pixelHeight - mousePosition.y * mult;
        mousePosition.x *= mult;

        Vector2 worldPoint = scene.camera.ScreenToWorldPoint(mousePosition);
        Debug.Log(mousePosition + " world " + worldPoint);

        _currentSelectedTile = TileMeshManager.GetTile(worldPoint);

        if (_currentSelectedTile == _previousTile && _previousTile != null)
        {
            return;
        }
        else
        {
            TileMeshManager.Draw(worldPoint, paintLayer, imageCollection);
            _previousTile = _currentSelectedTile;
        }
    }
}
