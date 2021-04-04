using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(TilesheetConfig))]
public class TilesheetConfigEditor : Editor
{

    TilesheetConfig tilesheetConfig = null;
    private List<Button> buttons;
    private Dictionary<TilesheetCaseTile, TextElement> actuals;

    private VisualTreeAsset tileButtonTemplate;
    private VisualTreeAsset tileTemplate;
    private VisualTreeAsset caseTemplate;
    private VisualTreeAsset mainTemplate;
    private VisualTreeAsset previewImageTemplate;

    private float sliderLeftPadding = 40;
    private float padding = 10;

    private int previewWidth = 10;
    private int previewHeight = 10;
    private VisualElement container;

    RandomNumberGenerator rng;


    public override VisualElement CreateInspectorGUI()
    {
        tilesheetConfig = (TilesheetConfig)target;
        container = new VisualElement();
        LoadTemplates();
        DrawMainUI();
        if (tilesheetConfig.Cases != null && tilesheetConfig.Texture != null) {
            rng = new RandomNumberGenerator("randomseed");
            DrawTilesheetUI();
        }
        return container;
    }

    private void LoadTemplates() {
        tileButtonTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/TilesheetCaseButton.uxml");
        tileTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/TilesheetCaseTile.uxml");
        caseTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/TilesheetCase.uxml");
        mainTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/TilesheetConfig.uxml");
        previewImageTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Resources/TilesheetCasePreviewImage.uxml");
    }

    private VisualElement DrawMainUI() {
        VisualElement main = new VisualElement();
        mainTemplate.CloneTree(main);

        main.Q<TextElement>(className: "c-name").text = tilesheetConfig.name;
        main.Q<Button>(className: "load-textures").clickable.clicked += () => {
            tilesheetConfig.LoadTextureCases();
            DrawTilesheetUI();
        };
        container.Add(main);
        return main;
    }

    private void DrawTilesheetUI() {
        DrawTileSheet();
        DrawTileSheetCases();
    }

    private void DrawTileSheet() {
        Image img = container.Q<Image>(className: "texture-display-img");
        img.style.width = tilesheetConfig.Texture.width;
        img.style.height = tilesheetConfig.Texture.height;
        img.image = tilesheetConfig.Texture;
    }

    private void DrawTileSheetCases() {
        buttons = new List<Button>();
        VisualElement textureContainer = container.Q<VisualElement>(className: "texture-display");
        VisualElement caseContainer = container.Q<VisualElement>(className: "case-display");
        foreach(TilesheetCase tilesheetCase in tilesheetConfig.Cases) {
            VisualElement buttonContainer = tileButtonTemplate.CloneTree();
            Button button = DrawTileSheetCaseButton(buttonContainer, tilesheetCase, (Button button) => {
                buttons.ForEach(button => button.RemoveFromClassList("active-button"));
                button.AddToClassList("active-button");
                DrawTilesheetCaseTile(tilesheetCase, caseContainer);
                DrawTileSheetPreview(tilesheetCase);
            });
            if (button != null) {
                buttons.Add(button);
                textureContainer.Add(button);
            }
        }
    }

    private void DrawTileSheetPreview(TilesheetCase tilesheetCase) {
        VisualElement previewContainer = container.Q<VisualElement>(className: "preview-container");
        previewContainer.Clear();
        previewContainer.style.width = previewWidth * tilesheetCase.Sprite.rect.width + 2;
        previewContainer.style.height = previewHeight * tilesheetCase.Sprite.rect.height + 2;
        for(int column = 0; column < previewWidth; column += 1) {
            for (int row = 0; row < previewHeight; row += 1) {
                DrawTileSheetPreviewImage(previewContainer, tilesheetCase, row, column);
            }
        }
    }

    private void DrawTileSheetPreviewImage(VisualElement previewContainer, TilesheetCase tilesheetCase, int row, int column) {
        VisualElement imageTemplate = previewImageTemplate.CloneTree();
        Image image = imageTemplate.Q<Image>();
        if (image == null) {
            Debug.Log($"Error! Couldn't find <Image> element in '{previewContainer.name}' ({previewContainer.childCount})!");
        }
        if (rng == null) {
            rng = new RandomNumberGenerator("randomseed");
        }
        Sprite sprite = tilesheetCase.Get(rng);

        Rect rect = GetSpriteRect(sprite);
        image.image = sprite.texture;
        image.style.width = rect.width;
        image.style.height = rect.height;
        image.style.left = rect.width * column;
        image.style.top = rect.height * row;
        image.sourceRect = rect;
        previewContainer.Add(image);
    }

    private Button DrawTileSheetCaseButton(VisualElement buttonContainer, TilesheetCase tilesheetCase, System.Action<Button> action) {
        Sprite sprite = tilesheetCase.Sprite;
        if (sprite == null) {
            Debug.Log($"Error! {tilesheetCase.CaseName} has no sprite!");
            return null;
        }
        Button button = buttonContainer.Q<Button>(className: "tile-sheet-case-button");
        Rect rect = GetSpriteRect(sprite);
        button.style.width = rect.width + 1;
        button.style.height = rect.height + 1;
        button.style.left = rect.x-1;
        button.style.top = rect.y-1;
        button.clickable.clicked += () => {action.Invoke(button);};
        return button;
    }

    private void DrawTilesheetCaseTile(TilesheetCase tilesheetCase, VisualElement caseContainer) {
        actuals = new Dictionary<TilesheetCaseTile, TextElement>();
        VisualElement caseElement = caseTemplate.CloneTree();
        TextElement title = caseElement.Q<TextElement>(className: "tilesheet-case-title");
        TextElement textLeft = caseElement.Q<TextElement>(className: "tilesheet-case-text-left");

        textLeft.style.left = tilesheetCase.Sprite.rect.width * 2 + padding * 2 + sliderLeftPadding;
        DrawTiles(tilesheetCase, caseContainer, caseElement);
        title.text = tilesheetCase.CaseName;

        Button addButton = caseElement.Q<Button>(className: "tile-list-add-button");
        addButton.clickable.clicked += () => {
            tilesheetCase.Tiles.Add(new TilesheetCaseTile(null, null, 0));
            DrawTilesheetCaseTile(tilesheetCase, caseContainer);
            DrawTileSheetPreview(tilesheetCase);
        };

        caseContainer.Clear();
        caseContainer.Add(caseElement);
    }

    private void DrawTiles(TilesheetCase tilesheetCase, VisualElement caseContainer, VisualElement caseElement) {
        VisualElement tileContainer = caseElement.Q<VisualElement>(className: "tile-container");
        int index = 0;
        TilesheetCaseTile firstTile = tilesheetCase.Tiles[0];
        foreach(TilesheetCaseTile tile in tilesheetCase.Tiles) {
            VisualElement uxCaseTile = tileTemplate.CloneTree();

            Rect rect = new Rect(firstTile.Sprite.rect);
            if (tile.Sprite != null) {
                rect = GetSpriteRect(tile.Sprite);
            }
            DrawTileImage(uxCaseTile, tile, rect);

            DrawSpritePicker(tilesheetCase, tile, uxCaseTile, caseContainer, rect);

            if (tile.Sprite != null) {
                TextElement actual = DrawActualSpawnChance(uxCaseTile, tile, rect.width * 2 + padding * 2);
                if (!actuals.ContainsKey(tile)) {
                    actuals.Add(tile, actual);
                }
                DrawTileSlider(uxCaseTile, tilesheetCase, tile, actual, rect.width * 2 + padding + sliderLeftPadding);
            } else {
                SliderInt slider = uxCaseTile.Q<SliderInt>(className: "tilesheet-case-tile-slider");
                slider.RemoveFromHierarchy();
            }

            Button removeButton = uxCaseTile.Q<Button>(className: "tile-list-remove-button");
            if (index != 0) {
                removeButton.clickable.clicked += () => {
                    tilesheetCase.Tiles.Remove(tile);
                    DrawTilesheetCaseTile(tilesheetCase, caseContainer);
                    DrawTileSheetPreview(tilesheetCase);
                };
            } else {
                removeButton.RemoveFromHierarchy();
            }

            tileContainer.Add(uxCaseTile);
            index += 1;
        }
        tilesheetCase.CalculateSpawnChances();
        UpdateActuals();
    }

    private void DrawSpritePicker(TilesheetCase tilesheetCase, TilesheetCaseTile tile, VisualElement tileContainer, VisualElement caseContainer, Rect spriteRect) {
        ObjectField spritePicker = tileContainer.Q<ObjectField>(className: "tilesheet-case-sprite-picker");
        spritePicker.style.left = spriteRect.width + padding;
        spritePicker.style.height = spriteRect.height;
        spritePicker.style.width = spriteRect.width;
        spritePicker.RegisterCallback<ChangeEvent<Object>>(change => {
            tile.Sprite = (Sprite)change.newValue;
            DrawTilesheetCaseTile(tilesheetCase, caseContainer);
            DrawTileSheetPreview(tilesheetCase);
        }, TrickleDown.NoTrickleDown);
    }

    private void DrawTileImage(VisualElement tileContainer, TilesheetCaseTile tile, Rect rect) {
        Sprite sprite = tile.Sprite;
        Image image = tileContainer.Q<Image>(className: "tilesheet-case-tile-image");
        if (image == null) {
            Debug.Log($"Error! Couldn't find <Image> element in '{tileContainer.name}' ({tileContainer.childCount})!");
        }
        image.style.width = rect.width;
        image.style.height = rect.height;
        if (sprite != null) {
            image.image = sprite.texture;
            image.sourceRect = rect;
            image.MarkDirtyRepaint();
        }
    }

    private Rect GetSpriteRect(Sprite sprite) {
        Rect rect = new Rect(sprite.rect);
        rect.y = sprite.texture.height - rect.y - rect.height;
        return rect;
    }

    private void UpdateActuals() {
        foreach(KeyValuePair<TilesheetCaseTile, TextElement> pair in actuals) {
            pair.Value.text = $"{pair.Key.ActualSpawnChance}%";
        }
    }

    private TextElement DrawActualSpawnChance(VisualElement tileContainer, TilesheetCaseTile tile, float posLeft) {
        TextElement actual = tileContainer.Q<TextElement>(className: "tilesheet-case-tile-actual-chance");
        actual.text = $"{tile.ActualSpawnChance}%";
        actual.style.left = posLeft;
        return actual;
    }

    private void DrawTileSlider(VisualElement sliderContainer, TilesheetCase tilesheetCase, TilesheetCaseTile tile, TextElement actual, float posLeft) {
        SliderInt slider = sliderContainer.Q<SliderInt>(className: "tilesheet-case-tile-slider");
        slider.value = tile.Distribution;
        slider.lowValue = tilesheetCase.DistributionMin;
        slider.highValue = tilesheetCase.DistributionMax;
        slider.RegisterValueChangedCallback(value => {
            tile.Distribution = value.newValue;
            tilesheetCase.CalculateSpawnChances();
            UpdateActuals();
            DrawTileSheetPreview(tilesheetCase);
        });
        slider.style.left = posLeft;
    }


}