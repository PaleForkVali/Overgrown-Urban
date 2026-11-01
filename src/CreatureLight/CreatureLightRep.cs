using DevInterface;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OvergrownUrban.CreatureLight
{
    public class CreatureLightRep : PlacedObjectRepresentation, IDevUISignals
    {
        public class CreatureLightPanel : Panel
        {
            public class PagedPanel : Panel, IDevUISignals
            {
                public class PagedPanelButton : Button
                {
                    public PagedPanel panel;

                    public int page;

                    private bool _enabled;

                    public bool enabled
                    { 
                        get
                        {
                            return _enabled;
                        }
                        set
                        {
                            _enabled = value;

                            for (int i = 0; i < fSprites.Count; i++)
                            {
                                fSprites[i].isVisible = value;
                            }

                            for (int i = 0; i < fLabels.Count; i++)
                            {
                                fLabels[i].isVisible = value;
                            }
                        }
                    }

                    public PagedPanelButton(DevUI owner, PagedPanel panel, int page, Vector2 pos, string title) : base(owner, panel.IDstring + "_Button", panel, pos, panel.size.x - 10f, title)
                    {
                        this.panel = panel;
                        this.page = page;
                    }

                    public override void Update()
                    {
                        if (enabled)
                        {
                            base.Update();
                        }
                    }

                    public override void Refresh()
                    {
                        if (enabled)
                        {
                            base.Refresh();

                            if (fLabels != null && fLabels .Count > 0 && fLabels[0] != null)
                            {
                                MoveLabel(0, absPos + new Vector2(size.x / 2f, 1f));
                                fLabels[0].alignment = FLabelAlignment.Center;
                            }
                        }
                    }
                }

                string title;

                public int page = 0;

                public List<PagedPanelButton[]> buttons;

                public Button prev;

                public Button next;

                public float pageTop => size.y - 40f;

                public int buttonsPerPage => Mathf.FloorToInt(size.y / 25f);

                public PagedPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name) : base(owner, IDstring, parentNode, pos, new Vector2(250f, 225f), name)
                {
                    title = name;
                    subNodes.Add(prev = new Button(owner, IDstring + "_prev", this, new Vector2(5f, size.y - 20f), 20f, "<"));
                    subNodes.Add(next = new Button(owner, IDstring + "_next", this, new Vector2(size.x - 25f, size.y - 20f), 20f, ">"));

                    buttons = new List<PagedPanelButton[]>();
                    InitPages();

                    fSprites.Add(new FSprite("pixel") { anchorY = 0f });
                    owner.placedObjectsContainer.AddChild(fSprites[fSprites.Count - 1]);

                    fSprites.Add(new FSprite("pixel"));
                    Futile.stage.AddChild(fSprites[fSprites.Count - 1]);
                }

                public virtual void InitPages()
                {
                    if (buttons.Count == 0)
                    {
                        buttons.Add(new PagedPanelButton[buttonsPerPage]);
                    }

                    (int page, int index) p = FirstEmptySlot();
                    buttons[p.page][p.index] = new PagedPanelButton(owner, this, p.index, new Vector2(5f, Mathf.Lerp(pageTop - 5f, 5f, (float)p.index / (float)(buttonsPerPage - 1))), "+");
                    buttons[p.page][p.index].enabled = page == p.page;
                    subNodes.Add(buttons[p.page][p.index]);
                }

                public (int page, int index) FirstEmptySlot()
                {
                    for (int i = 0; i < buttons.Count; i++)
                    {
                        if (FirstEmptySlotInPage(i).index != -1)
                        {
                            return FirstEmptySlotInPage(i);
                        }
                    }

                    buttons.Add(new PagedPanelButton[buttonsPerPage]);
                    return (buttons.Count - 1, 0);
                }

                public (int page, int index) FirstEmptySlotInPage(int page)
                {
                    if (buttons[page][buttons[page].Length - 1] == null)
                    {
                        if (buttons[page].IndexOf(buttons[page].First(x => x is null)) < buttonsPerPage)
                        {
                            return (page, buttons[page].IndexOf(buttons[page].First(x => x is null)));
                        }
                    }

                    return (page, -1);
                }

                public void RefreshPages(bool softRefresh = true)
                {
                    if (softRefresh)
                    {
                        for (int p = 0; p < buttons.Count; p++)
                        {
                            for (int i = 0; i < buttons[p].Length; i++)
                            {
                                if (buttons[p][i] == null) continue;

                                buttons[p][i].enabled = page == p;
                            }
                        }
                    }
                    else
                    {
                        for (int p = 0; p < buttons.Count; p++)
                        {
                            for (int i = 0; i < buttons[p].Length; i++)
                            {
                                if (buttons[p][i] == null) continue;

                                buttons[p][i].enabled = false;
                            }
                        }

                        page = 0;
                        buttons.Clear();
                        InitPages();
                    }
                }

                public virtual void ButtonClicked(PagedPanelButton button)
                {

                }

                public virtual void AddItemButtonClicked()
                {

                }

                public override void Refresh()
                {
                    if (fSprites.Count > 0 && fLabels.Count > 0)
                    {
                        base.Refresh();

                        MoveSprite(fSprites.Count - 2, absPos);
                        fSprites[fSprites.Count - 2].scaleY = Custom.Dist(absPos, (parentNode as Panel).absPos);
                        fSprites[fSprites.Count - 2].rotation = Custom.AimFromOneVectorToAnother(absPos, (parentNode as Panel).absPos);

                        MoveSprite(fSprites.IndexOf(fSprites[fSprites.Count - 1]), absPos + new Vector2(size.x / 2f, size.y - 25f));
                        fSprites[fSprites.Count - 1].scaleX = size.x;

                        fLabels[0].text = title + " (" + (page + 1) + "/" + buttons.Count + ")";

                        prev.MoveLabel(0, prev.absPos + new Vector2(prev.size.x / 2f, 1f));
                        prev.fLabels[0].alignment = FLabelAlignment.Center;

                        next.MoveLabel(0, next.absPos + new Vector2(next.size.x / 2f, 1f));
                        next.fLabels[0].alignment = FLabelAlignment.Center;
                    }
                }

                public void Signal(DevUISignalType type, DevUINode sender, string message)
                {
                    if (sender == prev)
                    {
                        if (page > 0)
                        {
                            page--;
                        }

                        RefreshPages();
                    }
                    else if (sender == next)
                    {
                        if (page < buttons.Count - 1)
                        {
                            page++;
                        }

                        RefreshPages();
                    }
                    else if (sender.IDstring.Contains(this.IDstring))
                    {
                        if ((sender as PagedPanelButton).Text == "+")
                        {
                            AddItemButtonClicked();
                        }
                        else
                        {
                            ButtonClicked(sender as PagedPanelButton);
                        }
                    }
                }
            }

            public class TriggerPanel : PagedPanel
            {
                public class CritPanel : PagedPanel
                {
                    public CritPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name) : base(owner, IDstring, parentNode, pos, name)
                    {

                    }

                    public override void InitPages()
                    {
                        int neededPages = Mathf.CeilToInt((float)(CreatureTemplate.Type.values.Count) / (float)buttonsPerPage);

                        for (int p = 0; p < neededPages; p++)
                        {
                            PagedPanelButton[] currentPage = new PagedPanelButton[buttonsPerPage];
                            for (int i = 0; i < buttonsPerPage; i++)
                            {
                                if ((p * (buttonsPerPage + 1)) + i >= CreatureTemplate.Type.values.Count)
                                {
                                    break;
                                }

                                currentPage[i] = new PagedPanelButton(owner, this, p, new Vector2(5f, Mathf.Lerp(pageTop - 5f, 5f, (float)i / (float)(buttonsPerPage - 1))), CreatureTemplate.Type.values.GetEntry((p * ((buttonsPerPage + 1) - 1)) + i));
                                currentPage[i].enabled = page == p;
                                subNodes.Add(currentPage[i]);
                            }

                            buttons.Add(currentPage);
                        }
                    }

                    public override void ButtonClicked(PagedPanelButton button)
                    {
                        CreatureTemplate.Type temp = ExtEnum<CreatureTemplate.Type>.Parse(typeof(CreatureTemplate.Type), button.Text, true) as CreatureTemplate.Type;

                        if (!(parentNode as TriggerPanel).data.creaturesToDetect.Contains(temp))
                        {
                            (parentNode as TriggerPanel).data.creaturesToDetect.Add(temp);
                            (parentNode as TriggerPanel).RefreshPages(false);
                        }
                    }
                }

                public CritPanel critPanel;

                public CreatureLightData data => (parentNode as CreatureLightPanel).data;

                public TriggerPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name) : base(owner, IDstring, parentNode, pos, name)
                {

                }

                public override void InitPages()
                {
                    int neededPages = Mathf.CeilToInt((float)(data.creaturesToDetect.Count) / (float)buttonsPerPage);
                    data.creaturesToDetect.Sort(CreatureLightData.SortCritList);

                    for (int p = 0; p < neededPages; p++)
                    {
                        PagedPanelButton[] currentPage = new PagedPanelButton[buttonsPerPage];
                        for (int i = 0; i < buttonsPerPage; i++)
                        {
                            if ((p * (buttonsPerPage + 1)) + i >= data.creaturesToDetect.Count)
                            {
                                break;
                            }

                            currentPage[i] = new PagedPanelButton(owner, this, p, new Vector2(5f, Mathf.Lerp(pageTop - 5f, 5f, (float)i / (float)(buttonsPerPage - 1))), data.creaturesToDetect[(p * (buttonsPerPage + 1)) + i].ToString());
                            currentPage[i].enabled = page == p;
                            subNodes.Add(currentPage[i]);
                        }

                        buttons.Add(currentPage);
                    }

                    base.InitPages();
                }

                public override void ButtonClicked(PagedPanelButton button)
                {
                    data.creaturesToDetect.Remove(ExtEnum<CreatureTemplate.Type>.Parse(typeof(CreatureTemplate.Type), button.Text, true) as CreatureTemplate.Type);
                    RefreshPages(false);
                }

                public override void AddItemButtonClicked()
                {
                    if (critPanel == null)
                    {
                        critPanel = new CritPanel(owner, IDstring + "_CritPanel", this, new Vector2(size.x + 10f, 0f), "Creature Type");
                        subNodes.Add(critPanel);
                    }
                    else
                    {
                        critPanel.ClearSprites();
                        critPanel = null;
                    }
                }
            }

            public class ColourPanel : Panel
            {
                public CreatureLightData data => (parentNode as CreatureLightPanel).data;

                public int dataIndex;

                public Button colourTypeButton;

                public SignalSlider redSlider;

                public SignalSlider greenSlider;

                public SignalSlider blueSlider;

                public Color color
                { 
                    get
                    {
                        PlacedObject.LightSourceData.ColorType type = data.colorType[dataIndex];
                        RoomCamera rCam = owner.room.game.cameras[0];

                        if (rCam == null || type == PlacedObject.LightSourceData.ColorType.White)
                        {
                            data.colour[dataIndex] = Color.white;
                        }

                        if (type == PlacedObject.LightSourceData.ColorType.Environment)
                        {
                            data.colour[dataIndex] = rCam.PixelColorAtCoordinate(((parentNode as CreatureLightPanel).parentNode as CreatureLightRep).pObj.pos);
                        }
                        else if (type == PlacedObject.LightSourceData.ColorType.EffectColor1)
                        {
                            data.colour[dataIndex] = rCam.currentPalette.texture.GetPixel(30, 5);
                        }
                        else if (type == PlacedObject.LightSourceData.ColorType.EffectColor2)
                        {
                            data.colour[dataIndex] = rCam.currentPalette.texture.GetPixel(30, 3);
                        }

                        return data.colour[dataIndex];
                    }
                }

                public ColourPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name, int dataIndex) : base(owner, IDstring, parentNode, pos, new Vector2(250f, 145f), name)
                {
                    this.dataIndex = dataIndex;

                    subNodes.Add(colourTypeButton = new Button(owner, "ColourPanel_Button" + (dataIndex == 0 ? "A" : "B"), this, new Vector2(5f, 120f), 240f, data.colorType[dataIndex] == null ? "Custom" : data.colorType[dataIndex].ToString()));
                    subNodes.Add(redSlider = new SignalSlider(owner, "ColourPanel_Slider_r" + (dataIndex == 0 ? "A" : "B"), this, new Vector2(5f, 100f), "Red:", false, 110f, color.r));
                    subNodes.Add(greenSlider = new SignalSlider(owner, "ColourPanel_Slider_g" + (dataIndex == 0 ? "A" : "B"), this, new Vector2(5f, 80f), "Green:", false, 110f, color.g));
                    subNodes.Add(blueSlider = new SignalSlider(owner, "ColourPanel_Slider_b" + (dataIndex == 0 ? "A" : "B"), this, new Vector2(5f, 60f), "Blue:", false, 110f, color.b));

                    fSprites.Add(new FSprite("pixel") { anchorX = 0f, anchorY = 0f, scaleX = 240f, scaleY = 50f });
                    Futile.stage.AddChild(fSprites[fSprites.Count - 1]);

                    fSprites.Add(new FSprite("pixel") { anchorY = 0f });
                    owner.placedObjectsContainer.AddChild(fSprites[fSprites.Count - 1]);
                }

                public override void Refresh()
                {
                    base.Refresh();

                    colourTypeButton.Text = data.colorType[dataIndex] == null ? "Custom" : data.colorType[dataIndex].ToString();
                    redSlider.RefreshNubPos(color.r); redSlider.NumberText = (Mathf.Round(color.r * 100f) / 100f).ToString();
                    greenSlider.RefreshNubPos(color.g); greenSlider.NumberText = (Mathf.Round(color.g * 100f) / 100f).ToString();
                    blueSlider.RefreshNubPos(color.b); blueSlider.NumberText = (Mathf.Round(color.b * 100f) / 100f).ToString();

                    MoveSprite(fSprites.Count - 2, absPos + new Vector2(5f, 5f));
                    fSprites[fSprites.Count - 2].color = color;

                    MoveSprite(fSprites.Count - 1, absPos);
                    fSprites[fSprites.Count - 1].scaleY = Custom.Dist(absPos, (parentNode as Panel).absPos);
                    fSprites[fSprites.Count - 1].rotation = Custom.AimFromOneVectorToAnother(absPos, (parentNode as Panel).absPos);
                }
            }

            public CreatureLightData data => (parentNode as CreatureLightRep).data;

            public ColourPanel colourPanelA;

            public ColourPanel colourPanelB;

            public TriggerPanel triggerPanel;

            public FSprite divider1;

            public FSprite divider2;

            public FLabel labelA;

            public FLabel labelB;

            public CreatureLightPanel(DevUI owner, string IDstring, DevUINode parentNode, Vector2 pos, string name) : base(owner, IDstring, parentNode, pos, new Vector2(250f, 245f), name)
            {
                subNodes.Add(new Button(owner, "Detector_Button", this, new Vector2(5f, 225f), 240f, "Triggers"));
                subNodes.Add(new SignalSlider(owner, "DetectorRad_Slider", this, new Vector2(5f, 205f), "Inner Detect Rad: ", false, 110f, data.innerDetectionRad) { NumberText = (Mathf.Round(data.innerDetectionRad * 100f) / 100f).ToString() });

                for (int i = 0; i < 2; i++)
                {
                    float y = 65f + ((35f + 65f) * (1f - i));
                    string t = i == 0 ? "A" : "B";

                    subNodes.Add(new SignalSlider(owner, "Strength_Slider" + t, this, new Vector2(5f, y), "Strength: ", false, 110f, data.strength[i]) { NumberText = Mathf.Round(data.strength[i] * 100f) + "%" });
                    subNodes.Add(new Button(owner, "Color_Button" + t, this, new Vector2(5f, y - 20f), 100f, "Colour"));
                    subNodes.Add(new Button(owner, "Fade_With_Sun_Button" + t, this, new Vector2(125f, y - 20f), 50f, data.fadeWithSun[i] ? "Sun" : "Static"));
                    subNodes.Add(new Button(owner, "Flat_Button" + t, this, new Vector2(180f, y - 20f), 50f, data.flat[i] ? "Flat: ON" : "Flat: OFF"));
                    subNodes.Add(new SignalSlider(owner, "BlinkRate_Slider" + t, this, new Vector2(5f, y - 40f), "Blink Rate: ", false, 110f, data.blinkRate[i]) { NumberText = Mathf.Round(data.strength[i] * 100f) + "%" });
                    subNodes.Add(new Button(owner, "BlinkType_Button" + t, this, new Vector2(5f, y - 60f), 100f, data.blinkType[i].ToString()));
                    subNodes.Add(new Button(owner, "NightLight_Button" + t, this, new Vector2(125f, y - 60f), 100f, data.nightLight[i] ? "Night Only" : "Always On"));
                }
                    
                fSprites.Add(divider1 = new FSprite("pixel"));
                Futile.stage.AddChild(divider1);

                fSprites.Add(divider2 = new FSprite("pixel"));
                Futile.stage.AddChild(divider2);

                fLabels.Add(labelA = new FLabel(Custom.GetFont(), "LightA") { alignment = FLabelAlignment.Left });
                Futile.stage.AddChild(labelA);

                fLabels.Add(labelB = new FLabel(Custom.GetFont(), "LightB") { alignment = FLabelAlignment.Left });
                Futile.stage.AddChild(labelB);
            }

            public override void Refresh()
            {
                base.Refresh();

                MoveSprite(fSprites.IndexOf(divider1), absPos + new Vector2(size.x / 2f, 200f));
                divider1.scaleX = size.x;
                divider1.color = Color.white;

                MoveLabel(fLabels.IndexOf(labelA), absPos + new Vector2(5f, 190f));
                labelA.color = Color.white;

                MoveSprite(fSprites.IndexOf(divider2), absPos + new Vector2(size.x / 2f, 100f));
                divider2.scaleX = size.x;
                divider2.color = Color.white;

                MoveLabel(fLabels.IndexOf(labelB), absPos + new Vector2(5f, 90f));
                labelB.color = Color.white;
            }
        }

        public CreatureLightPanel panel;

        public Handle lightHandle;

        public Handle detectionHandle;

        public CreatureLightData data => pObj.data as CreatureLightData;

        public CreatureLightRep(DevUI owner, string IDstring, DevUINode parentNode, PlacedObject pObj, string name) : base(owner, IDstring, parentNode, pObj, name)
        {
            subNodes.Add(panel = new CreatureLightPanel(owner, "CreatureLightPanel", this, data.panelPos, "Creature Light"));

            subNodes.Add(lightHandle = new Handle(owner, "lightHandle", this, data.lightHandle));

            fSprites.Add(new FSprite("Futile_White") { shader = owner.room.game.rainWorld.Shaders["VectorCircle"] });
            fSprites.Add(new FSprite("pixel") { anchorY = 0f });
            fLabels.Add(new FLabel(Custom.GetFont(), "Light rad"));
            owner.placedObjectsContainer.AddChild(fSprites[1]);
            owner.placedObjectsContainer.AddChild(fSprites[2]);
            owner.placedObjectsContainer.AddChild(fLabels[1]);

            subNodes.Add(detectionHandle = new Handle(owner, "detectionHandle", this, data.detectorHandle));

            fSprites.Add(new FSprite("Futile_White") { shader = owner.room.game.rainWorld.Shaders["VectorCircle"], color = Color.magenta });
            fSprites.Add(new FSprite("pixel") { anchorY = 0f });
            fLabels.Add(new FLabel(Custom.GetFont(), "Detection rad"));
            owner.placedObjectsContainer.AddChild(fSprites[3]);
            owner.placedObjectsContainer.AddChild(fSprites[4]);
            owner.placedObjectsContainer.AddChild(fLabels[2]);

            fSprites.Add(new FSprite("pixel") { anchorY = 0f });
            owner.placedObjectsContainer.AddChild(fSprites[5]);

            fSprites.Add(new FSprite("Futile_White") { shader = owner.room.game.rainWorld.Shaders["VectorCircle"], color = Color.magenta });
            owner.placedObjectsContainer.AddChild(fSprites[6]);
        }

        public override void Refresh()
        {
            base.Refresh();

            MoveSprite(1, absPos);
            fSprites[1].scale = lightHandle.pos.magnitude / 8f;
            fSprites[1].alpha = 2f / lightHandle.pos.magnitude;

            MoveSprite(2, absPos);
            fSprites[2].scaleY = lightHandle.pos.magnitude;
            fSprites[2].rotation = Custom.AimFromOneVectorToAnother(absPos, lightHandle.absPos);
            data.lightHandle = lightHandle.pos;

            MoveLabel(1, lightHandle.absPos + new Vector2(0f, 15f));
            fLabels[1].isVisible = lightHandle.MouseOver;
            fLabels[1].color = lightHandle.fSprites[0].color;


            MoveSprite(3, absPos);
            fSprites[3].scale = detectionHandle.pos.magnitude / 8f;
            fSprites[3].alpha = 2f / detectionHandle.pos.magnitude;

            MoveSprite(4, absPos);
            fSprites[4].scaleY = detectionHandle.pos.magnitude;
            fSprites[4].rotation = Custom.AimFromOneVectorToAnother(absPos, detectionHandle.absPos);
            data.detectorHandle = detectionHandle.pos;

            MoveLabel(2, detectionHandle.absPos + new Vector2(0f, 15f));
            fLabels[2].isVisible = detectionHandle.MouseOver;
            fLabels[2].color = detectionHandle.fSprites[0].color;


            MoveSprite(5, absPos);
            fSprites[5].scaleY = panel.pos.magnitude;
            fSprites[5].rotation = Custom.AimFromOneVectorToAnother(absPos, panel.absPos);

            MoveSprite(6, absPos);
            fSprites[6].scale = (detectionHandle.pos.magnitude * data.innerDetectionRad) / 8f;
            fSprites[6].alpha = 2f / (detectionHandle.pos.magnitude * data.innerDetectionRad);
        }

        public void Signal(DevUISignalType type, DevUINode sender, string message)
        {
            string id = sender.IDstring;

            if (id.Contains("Detector_Button"))
            {
                if (panel.triggerPanel == null)
                {
                    panel.subNodes.Add(panel.triggerPanel = new CreatureLightPanel.TriggerPanel(owner, "TriggerPanel", panel, new Vector2(260f, 0f), "Creatures To Detect"));
                }
                else
                {
                    panel.triggerPanel.ClearSprites();
                    panel.subNodes.Remove(panel.triggerPanel);
                    panel.triggerPanel = null;
                }
            }

            if (id.Contains("Strength_Slider"))
            {
                data.strength[id.EndsWith("A") ? 0 : 1] = float.Parse(message, System.Globalization.NumberStyles.Float);
                (sender as SignalSlider).NumberText = Mathf.Round(data.strength[id.EndsWith("A") ? 0 : 1] * 100f) + "%";
            }

            if (id.Contains("DetectorRad_Slider"))
            {
                data.innerDetectionRad = float.Parse(message, System.Globalization.NumberStyles.Float);
                (sender as SignalSlider).NumberText = (Mathf.Round(data.innerDetectionRad * 100f) / 100f).ToString();
            }

            if (id.Contains("Color_Button"))
            {
                if (id.EndsWith("A"))
                {
                    if (panel.colourPanelA == null)
                    {
                        panel.subNodes.Add(panel.colourPanelA = new CreatureLightPanel.ColourPanel(owner, "ColourPanelA", panel, new Vector2(-260f, 0f), "LightA Colour", 0));
                    }
                    else
                    {
                        panel.colourPanelA.ClearSprites();
                        panel.subNodes.Remove(panel.colourPanelA);
                        panel.colourPanelA = null;
                    }
                }
                else
                {
                    if (panel.colourPanelB == null)
                    {
                        panel.subNodes.Add(panel.colourPanelB = new CreatureLightPanel.ColourPanel(owner, "ColourPanelB", panel, new Vector2(-260f, 0f), "LightB Colour", 1));
                    }
                    else
                    {
                        panel.colourPanelB.ClearSprites();
                        panel.subNodes.Remove(panel.colourPanelB);
                        panel.colourPanelB = null;
                    }
                }
            }

            if (id.StartsWith("ColourPanel"))
            {
                if (id.Contains("_Button"))
                {
                    if (data.colorType[id.EndsWith("A") ? 0 : 1] == null)
                    {
                        data.colorType[id.EndsWith("A") ? 0 : 1] = PlacedObject.LightSourceData.ColorType.Environment;
                    }
                    else
                    {
                        int num = (int)Custom.Mod(data.colorType[id.EndsWith("A") ? 0 : 1].Index + 1, ExtEnum<PlacedObject.LightSourceData.ColorType>.values.Count);
                        data.colorType[id.EndsWith("A") ? 0 : 1] = new PlacedObject.LightSourceData.ColorType(ExtEnum<PlacedObject.LightSourceData.ColorType>.values.GetEntry(num));
                    }
                }

                if (id.Contains("_Slider_r"))
                {
                    data.colour[id.EndsWith("A") ? 0 : 1].r = float.Parse(message, System.Globalization.NumberStyles.Float);
                    data.colorType[id.EndsWith("A") ? 0 : 1] = null;
                }

                if (id.Contains("_Slider_g"))
                {
                    data.colour[id.EndsWith("A") ? 0 : 1].g = float.Parse(message, System.Globalization.NumberStyles.Float);
                    data.colorType[id.EndsWith("A") ? 0 : 1] = null;
                }

                if (id.Contains("_Slider_b"))
                {
                    data.colour[id.EndsWith("A") ? 0 : 1].b = float.Parse(message, System.Globalization.NumberStyles.Float);
                    data.colorType[id.EndsWith("A") ? 0 : 1] = null;
                }
            }

            if (id.Contains("Fade_With_Sun_Button"))
            {
                data.fadeWithSun[id.EndsWith("A") ? 0 : 1] = !data.fadeWithSun[id.EndsWith("A") ? 0 : 1];
                (sender as Button).Text = data.fadeWithSun[id.EndsWith("A") ? 0 : 1] ? "Sun" : "Static";
            }

            if (id.Contains("Flat_Button"))
            {
                data.flat[id.EndsWith("A") ? 0 : 1] = !data.flat[id.EndsWith("A") ? 0 : 1];
                (sender as Button).Text = data.flat[id.EndsWith("A") ? 0 : 1] ? "Flat: ON" : "Flat: OFF";
            }

            if (id.Contains("BlinkRate_Slider"))
            {
                data.blinkRate[id.EndsWith("A") ? 0 : 1] = float.Parse(message, System.Globalization.NumberStyles.Float);
                (sender as SignalSlider).NumberText = Mathf.Round(data.blinkRate[id.EndsWith("A") ? 0 : 1] * 100f) + "%";
            }

            if (id.Contains("BlinkType_Button"))
            {
                int num = (int)Custom.Mod(data.blinkType[id.EndsWith("A") ? 0 : 1].Index + 1, ExtEnum<PlacedObject.LightSourceData.BlinkType>.values.Count);
                data.blinkType[id.EndsWith("A") ? 0 : 1] = new PlacedObject.LightSourceData.BlinkType(ExtEnum<PlacedObject.LightSourceData.BlinkType>.values.GetEntry(num));
                (sender as Button).Text = data.blinkType[id.EndsWith("A") ? 0 : 1].ToString();
            }

            if (id.Contains("NightLight_Button"))
            {
                data.flat[id.EndsWith("A") ? 0 : 1] = !data.flat[id.EndsWith("A") ? 0 : 1];
                (sender as Button).Text = data.flat[id.EndsWith("A") ? 0 : 1] ? "Night Only" : "Always On";
            }
        }
    }
}
