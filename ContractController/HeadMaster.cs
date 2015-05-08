using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using KSP.IO;
using Contracts;
using Contracts.Parameters;
using Contracts.Agents;
using Contracts.Predicates;
using Contracts.Templates;
using Contracts.Agents.Mentalities;

namespace ContractController
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class HeadMaster : MonoBehaviour
    {

        //Per-type blocks and prefers

        ApplicationLauncherButton CCButton = null;
        private Rect mainGUI = new Rect(150, 150, 250, 500);
        private bool showGUI = false;
        int mainGUID;
        int prefEditGUID;
        Vector2 scrollPosition;
        Vector2 scrollPosition2;

        Type editingType;

        String minFundsAdvance = "0";
        String maxFundsAdvance = "150000";
        String minFundsComplete = "0";
        String maxFundsComplete = "150000";
        String minFundsFailure = "0";
        String maxFundsFailure = "150000";

        String minRepComplete = "0";
        String maxRepComplete = "100";
        String minRepFailure = "0";
        String maxRepFailure = "100";

        String minScienceComplete = "0";
        String maxScienceComplete = "100";

        String minParams = "0";
        String maxParams = "20";
        String minPrestiege = "1";
        String maxPrestiege = "3";

        bool showtypeprefeditor = false;

        bool displayed = false;
        bool done = false;
        bool inited = false;
        bool needsInit = true;

        //Contract variables
        List<Type> blockedTypes = new List<Type>();
        Dictionary<Type, TypePreference> typeMap = new Dictionary<Type, TypePreference>();
        Dictionary<TypePreference, List<String>> typePrefDict = new Dictionary<TypePreference, List<string>>();

        
        void Awake()
        {
            Debug.Log("*yawn* I'm awake");
            mainGUID = Guid.NewGuid().GetHashCode();
            prefEditGUID = Guid.NewGuid().GetHashCode();
            //Debug.Log("Active+Enabled?: " + ContractSystem.Instance.isActiveAndEnabled);
            done = false;


            InitButtons();
        }
        void FixedUpdate()
        {
            //Debug.Log("boop");
            //Debug.Log("Active+Enabled?: " + ContractSystem.Instance.isActiveAndEnabled);


            if (ContractSystem.Instance.isActiveAndEnabled && ContractSystem.Instance != null)
            {
                if (!inited)
                {
                    initTypePreferences();
                }
                if (!done)
                {

                    //Debug.Log("Contract types: " + Contracts.ContractSystem.ContractTypes);
                    List<Contract> toDelete = new List<Contract>();
                    List<Contract> toAccept = new List<Contract>();
                    if (Contracts.ContractSystem.Instance.Contracts != null)
                    {
                        foreach (Contract c in Contracts.ContractSystem.Instance.Contracts)
                        {
                            //check blocked types
                            //check blocked bodies
                            //check blocked strings
                            //check blocked agents
                            //check intParams
                            //check finances

                            if (checkForBlockedType(c)) { toDelete.Add(c); }
                            if (checkForBlockedAgent(c)) { toDelete.Add(c); }
                            if (checkForBlockedMentality(c)) { toDelete.Add(c); }
                            if (checkForBlockedParameters(c)) { toDelete.Add(c); }
                            if (checkForBlockedStrings(c)) { toDelete.Add(c); }
                            if (checkForBlockedBody(c)) { toDelete.Add(c); }

                            if (checkForMaxFundsAdvance(c)) { toDelete.Add(c); }
                            if (checkForMinFundsAdvance(c)) { toDelete.Add(c); }
                            if (checkForMinFundsCompletion(c)) { toDelete.Add(c); }
                            if (checkForMaxFundsCompletion(c)) { toDelete.Add(c); }
                            if (checkForMinFundsFailure(c)) { toDelete.Add(c); }
                            if (checkForMaxFundsFailure(c)) { toDelete.Add(c); }

                            if (checkForMinRepCompletion(c)) { toDelete.Add(c); }
                            if (checkForMaxRepCompletion(c)) { toDelete.Add(c); }
                            if (checkForMinRepFailure(c)) { toDelete.Add(c); }
                            if (checkForMaxRepFailure(c)) { toDelete.Add(c); }
                            if (checkForMinFundsCompletion(c)) { toDelete.Add(c); }

                            if (checkForMinScienceCompletion(c)) { toDelete.Add(c); }
                            if (checkForMaxScienceCompletion(c)) { toDelete.Add(c); }

                            if (checkForMinParams(c)) { toDelete.Add(c); }
                            if (checkForMaxParams(c)) { toDelete.Add(c); }

                            if (checkForWhitedAgent(c)) { toAccept.Add(c); }
                            if (checkForWhitedBody(c)) { toAccept.Add(c); }
                            if (checkForWhitedMentality(c)) { toAccept.Add(c); }
                            if (checkForWhitedParameter(c)) { toAccept.Add(c); }
                            if (checkForWhitedPrestiege(c)) { toAccept.Add(c); }
                            if (checkForWhitedString(c)) { toAccept.Add(c); }
                            if (checkForWhitedType(c)) { toAccept.Add(c); }

                        }
                    }
                    else
                    {
                        Debug.Log("Contract System is null");
                    }
                    removeBlackedContracts(toDelete);
                    acceptWhitedContracts(toAccept);
                    done = true;
                }
            }
        }
        void OnAppButtonReady()
        {
            Debug.Log("App launcher Ready");
            if (ApplicationLauncher.Ready)
            {
                Debug.Log("Doing Button things");
                CCButton = ApplicationLauncher.Instance.AddModApplication(
                    OnAppLaunchToggleOn,
                    OnAppLaunchToggleOff,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    DummyVoid,
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    (Texture)GameDatabase.Instance.GetTexture("ContractController/Textures/appButton", false));
            }
        }

        private void ShowGUI()
        {
            Debug.Log("ShowGUI");
            showGUI = true;
        }

        private void HideGUI()
        {
            Debug.Log("HideGUI");
            showGUI = false;
        }

        void OnAppLaunchToggleOff()
        {
            showGUI = false;
        }

        void OnAppLaunchToggleOn()
        {
            showGUI = true;
        }

        void DummyVoid() { }

        void InitButtons()
        {
            Debug.Log("Init Buttons");
            if (CCButton == null)
            {
                Debug.Log("AppButton Null, proceeding to ready.");
                GameEvents.onGUIApplicationLauncherReady.Add(OnAppButtonReady);
                if (ApplicationLauncher.Ready)
                {
                    OnAppButtonReady();
                }
            }

            GameEvents.onShowUI.Add(ShowGUI);
            GameEvents.onHideUI.Add(HideGUI);

            needsInit = false;
        }

        void DestroyButtons()
        {
            Debug.Log("Destroying Buttons");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnAppButtonReady);
            GameEvents.onShowUI.Remove(ShowGUI);
            GameEvents.onHideUI.Remove(HideGUI);

            if (CCButton != null)
                ApplicationLauncher.Instance.RemoveModApplication(CCButton);


            needsInit = true;
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy");
            DestroyButtons();
        }
        void Save(ConfigNode node)
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<HeadMaster>();
            config.SetValue("TypePreferences", typeMap);
            config.save();
        }
        void Load(ConfigNode node)
        {
            PluginConfiguration config = PluginConfiguration.CreateForType<HeadMaster>();
            config.load();
            typeMap = config.GetValue<Dictionary<Type, TypePreference>>("TypePreferences");
        }


        void OnGUI()
        {
            if (showGUI)
            {
                mainGUI = GUILayout.Window(mainGUID, mainGUI, mainGUIWindow, "Contract Controller~");
                if (showtypeprefeditor)
                {
                    Rect rect = new Rect(mainGUI.right, mainGUI.top, 250, 400);

                    rect = GUILayout.Window(prefEditGUID, rect, editTypePrefGUI, "" + editingType.Name);
                }
            }

        }
        public void mainGUIWindow(int windowID)
        {
            //float value = 100;
            //value = GUILayout.VerticalScrollbar(value, 5, 100, 0);
            GUILayout.Label("Choose the contract type you want to edit:");
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(175), GUILayout.Height(500));
            //mainGUI.position = scrollPosition;

            foreach (Type t in Contracts.ContractSystem.ContractTypes)
            {
                if (GUILayout.Button(t.Name))
                {
                    editingType = t;
                    showtypeprefeditor = true;

                }

            }
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        public void editTypePrefGUI(int windowID)
        {
            //minFunds
            TypePreference tp = typeMap[editingType];
            //maxFunds
            GUILayout.BeginVertical();
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, GUILayout.Width(225), GUILayout.Height(250));
            //mainGUI.position = scrollPosition;

            GUILayout.BeginHorizontal();
            GUILayout.Label("minFundsAdvance:");
            typePrefDict[tp][0] = GUILayout.TextField(typePrefDict[tp][0]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxFundsAdvance:");
            typePrefDict[tp][1] = GUILayout.TextField(typePrefDict[tp][1]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("minFundsComplete:");
            typePrefDict[tp][2] = GUILayout.TextField(typePrefDict[tp][2]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxFundsComplete:");
            typePrefDict[tp][3] = GUILayout.TextField(typePrefDict[tp][3]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("minFundsFailure:");
            typePrefDict[tp][4] = GUILayout.TextField(typePrefDict[tp][4]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxFundsFailure:");
            typePrefDict[tp][5] = GUILayout.TextField(typePrefDict[tp][5]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("minRepComplete:");
            typePrefDict[tp][6] = GUILayout.TextField(typePrefDict[tp][6]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxRepComplete:");
            typePrefDict[tp][7] = GUILayout.TextField(typePrefDict[tp][7]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("minRepFailure:");
            typePrefDict[tp][8] = GUILayout.TextField(typePrefDict[tp][8]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxRepFailure:");
            typePrefDict[tp][9] = GUILayout.TextField(typePrefDict[tp][9]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("minScienceComplete:");
            typePrefDict[tp][10] = GUILayout.TextField(typePrefDict[tp][10]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxScienceComplete:");
            typePrefDict[tp][11] = GUILayout.TextField(typePrefDict[tp][11]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("minParameters:");
            typePrefDict[tp][12] = GUILayout.TextField(typePrefDict[tp][12]);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("maxParameters:");
            typePrefDict[tp][13] = GUILayout.TextField(typePrefDict[tp][13]);
            GUILayout.EndHorizontal();


            GUILayout.EndScrollView();



            GUILayout.EndVertical();
            if (GUILayout.Button("Save"))
            {

                tp.minFundsAdvance = float.Parse(typePrefDict[tp][0]);
                tp.maxFundsAdvance = float.Parse(typePrefDict[tp][1]);
                tp.minFundsCompletion = float.Parse(typePrefDict[tp][2]);
                tp.maxFundsCompletion = float.Parse(typePrefDict[tp][3]);
                tp.minFundsFailure = float.Parse(typePrefDict[tp][4]);
                tp.maxFundsFailure = float.Parse(typePrefDict[tp][5]);

                tp.minRepCompletion = float.Parse(typePrefDict[tp][6]);
                tp.maxRepCompletion = float.Parse(typePrefDict[tp][7]);
                tp.minRepFailure = float.Parse(typePrefDict[tp][8]);
                tp.maxRepFailure = float.Parse(typePrefDict[tp][9]);

                tp.minScienceCompletion = float.Parse(typePrefDict[tp][10]);
                tp.maxScienceCompletion = float.Parse(typePrefDict[tp][11]);

                tp.minParams = int.Parse(typePrefDict[tp][12]);
                tp.maxParams = int.Parse(typePrefDict[tp][13]);

                
                Debug.Log("Settings Saved");
            }
            if (GUILayout.Button("Close"))
            {
                showtypeprefeditor = false;
            }
        }

        public void initTypePreferences()
        {
            Debug.Log("Initing type preferences");
            if(ContractSystem.ContractTypes != null)
            {
                foreach (Type t in ContractSystem.ContractTypes)
                {
                    Debug.Log("Adding Type: " + t.ToString());
                    TypePreference tp = TypePreference.getDefaultTypePreference();
                    typeMap.Add(t, tp);
                    List<String> stringList = new List<string>();

                    stringList.Add(minFundsAdvance);
                    stringList.Add(minFundsComplete);
                       
                    stringList.Add(minFundsFailure);
                    stringList.Add(maxFundsAdvance);
                    stringList.Add(maxFundsComplete);
                    stringList.Add(maxFundsFailure);

                    stringList.Add(minRepComplete);
                    stringList.Add(maxRepComplete);
                    stringList.Add(minRepFailure);
                    stringList.Add(maxRepFailure);

                    stringList.Add(minScienceComplete);
                    stringList.Add(maxScienceComplete);

                    stringList.Add(minParams);
                    stringList.Add(maxParams);
                    stringList.Add(minPrestiege);
                    stringList.Add(maxPrestiege);
                    typePrefDict.Add(tp,stringList);
                }
            }
            inited = true;
        }

        ///////////////////////////////////////////////

        public bool checkForMinFundsAdvance(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.FundsAdvance < tp.minFundsAdvance)
            {
                return true;
            }

            return false;
        }
        public bool checkForMaxFundsAdvance(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.FundsAdvance > tp.maxFundsAdvance)
            {
                return true;
            }

            return false;
        }
        public bool checkForMinFundsCompletion(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.FundsCompletion < tp.minFundsCompletion)
            {
                return true;
            }
            return false;
        }
        public bool checkForMaxFundsCompletion(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.FundsCompletion > tp.maxFundsCompletion)
            {
                return true;
            }
            return false;
        }
        public bool checkForMinFundsFailure(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.FundsFailure < tp.minFundsFailure)
            {
                return true;
            }
            return false;
        }
        public bool checkForMaxFundsFailure(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.FundsFailure > tp.maxFundsFailure)
            {
                return true;
            }
            return false;
        }

        ///////////////////////////////////////////////

        public bool checkForMinScienceCompletion(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ScienceCompletion < tp.minScienceCompletion)
            {
                return true;
            }
            return false;
        }
        public bool checkForMaxScienceCompletion(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ScienceCompletion > tp.maxScienceCompletion)
            {
                return true;
            }
            return false;
        }

        ///////////////////////////////////////////////

        public bool checkForMinRepCompletion(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ReputationCompletion < tp.minRepCompletion)
            {
                return true;
            }
            return false;
        }
        public bool checkForMaxRepCompletion(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ReputationCompletion > tp.maxRepCompletion)
            {
                return true;
            }
            return false;
        }
        public bool checkForMinRepFailure(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ReputationFailure < tp.minRepFailure)
            {
                return true;
            }
            return false;
        }
        public bool checkForMaxRepFailure(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ReputationCompletion > tp.minRepFailure)
            {
                return true;
            }
            return false;
        }

        ///////////////////////////////////////////////



        ///////////////////////////////////////////////

        public bool checkForBlockedBody(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (String s in tp.blockedBodies)
            {
                if (c.Description.Contains(s))
                {
                    return true;
                }
                if (c.Synopsys.Contains(s))
                {
                    return true;
                }
                if (c.Title.Contains(s))
                {
                    return true;
                }
                foreach (String wurd in c.Keywords)
                {
                    if (wurd.Contains(s))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool checkForBlockedType(Contract c)
        {
            if (blockedTypes.Contains(c.GetType()))
            {
                return true;
            }
            return false;
        }
        public bool checkForBlockedParameters(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (ContractParameter cp in tp.blockedParameters)
            {
                foreach (ContractParameter cP in c.AllParameters)
                {
                    if (cp == cP)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool checkForBlockedAgent(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (tp.blockedAgents.Contains(c.Agent))
            {
                return true;
            }
            return false;
        }
        public bool checkForBlockedStrings(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (String s in tp.blockedStrings)
            {
                if (c.Description.Contains(s))
                {
                    return true;
                }
                if (c.Synopsys.Contains(s))
                {
                    return true;
                }
                if (c.Title.Contains(s))
                {
                    return true;
                }
                foreach (String wurd in c.Keywords)
                {
                    if (wurd.Contains(s))
                    {
                        return true;
                    }
                }
            }
            //c.Description;
            //c.Notes;
            //c.Synopsys;
            //c.Title;
            //c.Keywords;
            return false;
        }
        public bool checkForBlockedMentality(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            foreach (AgentMentality am in c.Agent.Mentality)
            {
                if (tp.blockedMentalities.Contains(am))
                {
                    return true;
                }
            }
            return false;
        }

        ///////////////////////////////////////////////

        public bool checkForWhitedBody(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            return false;
        }
        public bool checkForWhitedParameter(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (ContractParameter cp in tp.autoParameters)
            {
                foreach (ContractParameter cP in c.AllParameters)
                {
                    if (cp == cP)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool checkForWhitedString(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (String s in tp.autoStrings)
            {
                if (checkForString(c, s))
                {
                    return true;
                }
            }
            return false;
        }
        public bool checkForWhitedPrestiege(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (Contract.ContractPrestige p in tp.autoPrestieges)
            {
                if (p == c.Prestige)
                {
                    return true;
                }
            }
            return false;
        }
        public bool checkForWhitedMentality(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (AgentMentality am in tp.autoMentalities)
            {
                foreach (AgentMentality aM in c.Agent.Mentality)
                    if (aM == am)
                    {
                        return true;
                    }
            }
            return false;
        }
        public bool checkForWhitedType(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (Type type in tp.autoTypes)
            {
                if (type == t)
                {
                    return true;
                }
            }

            return false;
        }
        public bool checkForWhitedAgent(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            foreach (Agent a in tp.autoAgents)
            {
                if (c.Agent == a)
                {
                    return true;
                }
            }
            return false;
        }

        ///////////////////////////////////////////////

        public bool checkForMinParams(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];

            if (c.ParameterCount < tp.minParams)
            {
                return true;
            }

            return false;
        }
        public bool checkForMaxParams(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            if (c.ParameterCount > tp.maxParams)
            {
                return true;
            }
            return false;
        }
        ///////////////////////////////////////////////

        public void removeBlackedContracts(List<Contract> contractList)
        {
            foreach (Contract c in contractList)
            {
                Debug.Log("Removing: " + c.Title);
                Contracts.ContractSystem.Instance.Contracts.Remove(c);
            }
        }
        public void acceptWhitedContracts(List<Contract> contractList)
        {
            foreach (Contract c in contractList)
            {
                Debug.Log("Accepting: " + c.Title);
                c.Accept();
            }
        }
        public bool checkForString(Contract c, String s)
        {
            if (c.Description.Contains(s))
            {
                return true;
            }
            if (c.Synopsys.Contains(s))
            {
                return true;
            }
            if (c.Title.Contains(s))
            {
                return true;
            }
            foreach (String wurd in c.Keywords)
            {
                if (wurd.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }
        public void disableType()
        {

        }
        public void disableParameter(ContractParameter cp)
        {
            cp.Disable();
            ContractPredicate cP;

        }

    }
}
