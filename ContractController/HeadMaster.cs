using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using KSP.IO;
using Contracts;
using Contracts.Parameters;
using Contracts.Agents;
using Contracts.Predicates;
using Contracts.Templates;
using Contracts.Agents.Mentalities;

using File = System.IO.File;
using Debug = UnityEngine.Debug;

namespace ContractController
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class HeadMaster : MonoBehaviour
    {

        //Per-type blocks and prefers

        ApplicationLauncherButton CCButton = null;
        private Rect mainGUI = new Rect(100, 100, 250, 625);

        private bool showGUI = true;
        int mainGUID;
        int prefEditGUID;
        int parameterGuid;
        int parameterListGuid;
        int autoParameterGuid;
        int autoParameterListGuid;
        int bodyGuid;
        int bodyListGuid;
        int autoBodyGuid;
        int autoBodyListGuid;
        int stringGuid;
        int stringListGuid;
        int autoStringGuid;
        int autoStringListGuid;

        int editAllGuid;
        int allparameterGuid;
        int allparameterListGuid;
        int allautoParameterGuid;
        int allautoParameterListGuid;
        int allbodyGuid;
        int allbodyListGuid;
        int allautoBodyGuid;
        int allautoBodyListGuid;
        int allstringGuid;
        int allstringListGuid;
        int allautoStringGuid;
        int allautoStringListGuid;

        Vector2 scrollPosition;
        Vector2 scrollPosition2;
        Vector2 scrollPosition3;
        Vector2 scrollPosition4;
        Vector2 scrollPosition5;
        Vector2 scrollPosition6;
        Vector2 scrollPosition7;
        Vector2 scrollPosition8;
        Vector2 scrollPosition9;
        Vector2 scrollPosition10;
        Vector2 scrollPosition11;
        Vector2 scrollPosition12;
        Vector2 scrollPosition13;
        Vector2 scrollPosition14;
        Vector2 scrollPosition15;
        Vector2 scrollPosition16;
        Vector2 scrollPosition17;
        Vector2 scrollPosition18;
        Vector2 scrollPosition19;
        Vector2 scrollPosition20;
        Vector2 scrollPosition21;

        Type editingType;

        String stringthing = "";
        String stringthing1 = "";

        bool showtypeprefeditor = false;
        bool showParameterChangeGUI = false;
        bool showParameterListGUI = false;
        bool showAutoParamGUI = false;
        bool showBodyBlacklistGUI = false;
        bool showBodyAutoListGUI = false;
        bool showStringBlacklistGUI = false;
        bool showStringWhitelistGUI = false;

        bool showAllParameterChangeGUI = false;
        bool showAllParameterListGUI = false;
        bool showAllAutoParamGUI = false;
        bool showAllBodyBlacklistGUI = false;
        bool showAllBodyAutoListGUI = false;
        bool showAllStringBlacklistGUI = false;
        bool showAllStringWhitelistGUI = false;

        public String minFundsAdvanceString = "0";
        public String maxFundsAdvanceString = "1000000";
        public String minFundsCompleteString = "0";
        public String maxFundsCompleteString = "1000000";
        public String minFundsFailureString = "0";
        public String maxFundsFailureString = "1000000";

        public String minRepCompleteString = "0";
        public String maxRepCompleteString = "1000000";
        public String minRepFailureString = "0";
        public String maxRepFailureString = "1000000";

        public String minScienceCompleteString = "0";
        public String maxScienceCompleteString = "1000000";

        public String minParamsString = "0";
        public String maxParamsString = "20";
        public String minPrestiegeString = "0";
        public String maxPrestiegeString = "2";

        List<Type> blockedParams = new List<Type>();
        List<Type> whiteParams = new List<Type>();
        List<String> blackBodies = new List<String>();
        List<String> whiteBodies = new List<String>();
        List<String> blackStrings = new List<String>();
        List<String> whiteStrings = new List<String>();

        bool showEditAllGUI;

        String statusString = "";
        double timeTillIdle = 15000;
        Stopwatch idleWatch = new Stopwatch();
        [Persistent]
        bool inited = false;

        bool buttonNeedsInit = true;
        bool isSorting = false;
        float sortTime = 0.02f;
        String sortTimeString = "0.02";
        int currentTime = 0;

        int maxContracts = 2;
        List<String> CCstrings = new List<string>();
        //Contract variables
        List<Type> blockedTypes = new List<Type>();
        Dictionary<Type, TypePreference> typeMap = new Dictionary<Type, TypePreference>();
        List<Type> parameters = new List<Type>();

        void Awake()
        {

            DontDestroyOnLoad(this);
            Debug.Log("*yawn* I'm awake");
            mainGUID = Guid.NewGuid().GetHashCode();
            prefEditGUID = Guid.NewGuid().GetHashCode();
            parameterGuid = Guid.NewGuid().GetHashCode();
            parameterListGuid = Guid.NewGuid().GetHashCode();
            autoParameterGuid = Guid.NewGuid().GetHashCode();
            autoParameterListGuid = Guid.NewGuid().GetHashCode();
            bodyGuid = Guid.NewGuid().GetHashCode();
            bodyListGuid = Guid.NewGuid().GetHashCode();
            autoBodyGuid = Guid.NewGuid().GetHashCode();
            autoBodyListGuid = Guid.NewGuid().GetHashCode();
            stringGuid = Guid.NewGuid().GetHashCode();
            stringListGuid = Guid.NewGuid().GetHashCode();
            autoStringGuid = Guid.NewGuid().GetHashCode();
            autoStringListGuid = Guid.NewGuid().GetHashCode();
            editAllGuid = Guid.NewGuid().GetHashCode();

            allparameterGuid = Guid.NewGuid().GetHashCode();
            allparameterListGuid = Guid.NewGuid().GetHashCode();
            allautoParameterGuid = Guid.NewGuid().GetHashCode();
            allautoParameterListGuid = Guid.NewGuid().GetHashCode();
            allbodyGuid = Guid.NewGuid().GetHashCode();
            allbodyListGuid = Guid.NewGuid().GetHashCode();
            allautoBodyGuid = Guid.NewGuid().GetHashCode();
            allautoBodyListGuid = Guid.NewGuid().GetHashCode();
            allstringGuid = Guid.NewGuid().GetHashCode();
            allstringListGuid = Guid.NewGuid().GetHashCode();
            allautoStringGuid = Guid.NewGuid().GetHashCode();
            allautoStringListGuid = Guid.NewGuid().GetHashCode();
            //Debug.Log("Active+Enabled?: " + ContractSystem.Instance.isActiveAndEnabled);
            //done = false;
            
            //check for DMagic contracts
            /*
            string contractAssemblyName = "";
            foreach (AssemblyLoader.LoadedAssembly loaded in AssemblyLoader.loadedAssemblies)
            {
                Debug.Log(loaded.assembly.FullName);
                if (loaded.assembly.GetName().Name == "ContractConfigurator")
                {
                    //Debug.Log("Gotcha Bitch!");
                    contractAssemblyName = loaded.assembly.FullName;
                }
            }
             * */
            String contractConfiguratorTypeName = "ContractConfigurator.ContractType";
            //Type contractType = Type.GetType("ContractConfigurator.ContractType");
            Type contractType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
                                                        .SingleOrDefault(t => t.FullName == contractConfiguratorTypeName);
            Debug.Log("isNull?");
	        if(contractType != null)
            {
                Debug.Log("nope");
                String CCTypeMethod = "AllValidContractTypeNames";
                PropertyInfo CConfigTypeProperty = contractType.GetProperty(CCTypeMethod);
                var names = CConfigTypeProperty.GetValue(null, null);

                IEnumerable<string> enumerableNames = (IEnumerable<string>)names;
                //Debug.Log("List: " + enumerableNames.Count());
                foreach(String s in enumerableNames)
                {
                    Debug.Log("CCType: " + s);
                    Type t = Type.GetType(s);
                    Debug.Log("t: " + t.ToString());
                    CCstrings.Add(s);
                }
                //Debug.Log("test");
            }

            

            if (buttonNeedsInit && HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {

                Debug.Log("Initing buttons");
                InitButtons();
            }
            //myLoad();
        }
        void FixedUpdate()
        {
            //Debug.Log("boop");
            //Debug.Log("Active+Enabled?: " + ContractSystem.Instance.isActiveAndEnabled);

            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                if(currentTime * Time.fixedDeltaTime >= sortTime)
                {
                    if (ContractSystem.Instance != null && ContractSystem.Instance.isActiveAndEnabled)
                    {
                        if (inited == false)
                        {
                            if (typeMap.Count == 0)
                            {

                                String[] files = System.IO.Directory.GetFiles(KSPUtil.ApplicationRootPath + "/GameData/ContractFilter/", "*.cfg");

                                if (files.Contains(KSPUtil.ApplicationRootPath + "/GameData/ContractFilter/settings.cfg"))
                                {
                                    initTypePreferences();
                                    myLoad();
                                }
                                else
                                {
                                    initTypePreferences();
                                }
                                idleWatch.Start();
                                //Debug.Log(typeMap.Count);
                                //myLoad();
                            }

                            inited = true;
                        }
                        //appbutton texture changes
                        if (isSorting && showGUI)
                        {
                            CCButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("ContractFilter/Textures/appButton4", false));
                        }
                        else if (isSorting && !showGUI)
                        {
                            CCButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("ContractFilter/Textures/appButton3", false));
                        }
                        else if (!isSorting && showGUI)
                        {
                            CCButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("ContractFilter/Textures/appButton5", false));
                        }
                        else
                        {
                            CCButton.SetTexture((Texture)GameDatabase.Instance.GetTexture("ContractFilter/Textures/appButton", false));
                        }
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            isSorting = !isSorting;
                        }

                        float facLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.MissionControl);
                        if (facLevel == 0)
                        {
                            maxContracts = 2;
                        }
                        else if (facLevel == 1)
                        {
                            maxContracts = 7;
                        }
                        int liveContracts = ContractSystem.Instance.GetActiveContractCount();

                        if (isSorting && facLevel != 2)
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


                                    if (checkForBlockedType(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of blocked type");
                                        toDelete.Add(c);
                                    }
                                    if (checkForBlockedParameters(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of blocked parameters");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinFundsAdvance(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of low fund payout in advance" + "(" + c.FundsAdvance + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxFundsAdvance(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of high fund payout in advance" + "(" + c.FundsAdvance + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinFundsCompletion(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of low funds payout on completion" + "(" + c.FundsCompletion + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxFundsCompletion(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of high funds payout on completion" + "(" + c.FundsCompletion + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinFundsFailure(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of low funds on failure" + "(" + c.FundsFailure + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxFundsFailure(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of high funds on failure" + "(" + c.FundsFailure + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinScienceCompletion(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of low science on completion" + "(" + c.ScienceCompletion + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxScienceCompletion(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of high science on completion" + "(" + c.ScienceCompletion + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinRepCompletion(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of low rep on completion" + "(" + c.ReputationCompletion + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxRepCompletion(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of high rep on completion" + "(" + c.ReputationCompletion + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinRepFailure(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of low rep on failure" + "(" + c.ReputationFailure + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxRepFailure(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of high rep on failure" + "(" + c.ReputationFailure + ")");
                                        toDelete.Add(c);
                                    }
                                    if (checkForBlockedAgent(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of blocked agent");
                                        toDelete.Add(c);
                                    }
                                    if (checkForBlockedMentality(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of blocked mentality");
                                        toDelete.Add(c);
                                    }
                                    if (checkForWhitedType(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited type");
                                        toAccept.Add(c);
                                    }
                                    if (checkForWhitedAgent(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited agent");
                                        toAccept.Add(c);
                                    }
                                    if (checkForWhitedBody(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited body");
                                        toAccept.Add(c);
                                    }
                                    if (checkForWhitedMentality(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited mentality");
                                        toAccept.Add(c);
                                    }
                                    if (checkForWhitedParameter(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited parameter");
                                        toAccept.Add(c);
                                    }
                                    if (checkForWhitedPrestiege(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited prestiege");
                                        toAccept.Add(c);
                                    }
                                    if (checkForWhitedString(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be accepted because of whited string");
                                        toAccept.Add(c);
                                    }
                                    if (checkForBlockedStrings(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of blocked string");
                                        toDelete.Add(c);
                                    }
                                    if (checkForBlockedBody(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of blocked body");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMinParams(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of min params");
                                        toDelete.Add(c);
                                    }
                                    if (checkForMaxParams(c))
                                    {
                                        Debug.Log("Adding: " + c.Title + " to be deleted because of max params");
                                        toDelete.Add(c);
                                    }
                                }
                            }
                            else
                            {
                                Debug.Log("Contract System is null");
                            }
                            removeBlackedContracts(toDelete);
                            acceptWhitedContracts(toAccept);
                        }
                        else if (isSorting && liveContracts < maxContracts)
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
                                    if (liveContracts + toAccept.Count <= maxContracts)
                                    {
                                        if (checkForBlockedType(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of blocked type");
                                            toDelete.Add(c);
                                        }
                                        if (checkForBlockedParameters(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of blocked parameters");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinFundsAdvance(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of low fund payout in advance" + "(" + c.FundsAdvance + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxFundsAdvance(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of high fund payout in advance" + "(" + c.FundsAdvance + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinFundsCompletion(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of low funds payout on completion" + "(" + c.FundsCompletion + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxFundsCompletion(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of high funds payout on completion" + "(" + c.FundsCompletion + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinFundsFailure(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of low funds on failure" + "(" + c.FundsFailure + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxFundsFailure(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of high funds on failure" + "(" + c.FundsFailure + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinScienceCompletion(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of low science on completion" + "(" + c.ScienceCompletion + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxScienceCompletion(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of high science on completion" + "(" + c.ScienceCompletion + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinRepCompletion(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of low rep on completion" + "(" + c.ReputationCompletion + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxRepCompletion(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of high rep on completion" + "(" + c.ReputationCompletion + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinRepFailure(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of low rep on failure" + "(" + c.ReputationFailure + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxRepFailure(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of high rep on failure" + "(" + c.ReputationFailure + ")");
                                            toDelete.Add(c);
                                        }
                                        if (checkForBlockedAgent(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of blocked agent");
                                            toDelete.Add(c);
                                        }
                                        if (checkForBlockedMentality(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of blocked mentality");
                                            toDelete.Add(c);
                                        }
                                        if (checkForWhitedType(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited type");
                                            toAccept.Add(c);
                                        }
                                        if (checkForWhitedAgent(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited agent");
                                            toAccept.Add(c);
                                        }
                                        if (checkForWhitedBody(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited body");
                                            toAccept.Add(c);
                                        }
                                        if (checkForWhitedMentality(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited mentality");
                                            toAccept.Add(c);
                                        }
                                        if (checkForWhitedParameter(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited parameter");
                                            toAccept.Add(c);
                                        }
                                        if (checkForWhitedPrestiege(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited prestiege");
                                            toAccept.Add(c);
                                        }
                                        if (checkForWhitedString(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be accepted because of whited string");
                                            toAccept.Add(c);
                                        }
                                        if (checkForBlockedStrings(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of blocked string");
                                            toDelete.Add(c);
                                        }
                                        if (checkForBlockedBody(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of blocked body");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMinParams(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of min params");
                                            toDelete.Add(c);
                                        }
                                        if (checkForMaxParams(c))
                                        {
                                            Debug.Log("Adding: " + c.Title + " to be deleted because of max params");
                                            toDelete.Add(c);
                                        }
                                    }
                                    removeBlackedContracts(toDelete);
                                    acceptWhitedContracts(toAccept);
                                }
                            }
                            else
                            {
                                Debug.Log("Contract System is null");
                            }
                        }
                    }
                    currentTime = 0;
                }
                else
                {
                    currentTime++;
                }
                
                if (idleWatch.ElapsedMilliseconds >= 25000 && !isSorting)
                {
                    statusString = "Idling";
                }
            }
            else
            {
                showGUI = false;

            }
        }
        void OnAppButtonReady()
        {
            //Note: do tracking station check
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER && CCButton == null)
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
                        (Texture)GameDatabase.Instance.GetTexture("ContractFilter/Textures/appButton", false));
                    showGUI = CCButton.toggleButton.Value;
                }
            }

        }

        private void ShowGUI()
        {
            Debug.Log("ShowGUI");
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                showGUI = true;
            }

        }

        private void HideGUI()
        {
            Debug.Log("HideGUI");
            showGUI = false;
        }

        void OnAppLaunchToggleOff()
        {
            showGUI = false;
            if (CCButton.toggleButton.Value != showGUI) { CCButton.toggleButton.Value = showGUI; }
        }

        void OnAppLaunchToggleOn()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                showGUI = true;
                if (CCButton.toggleButton.Value != showGUI) { CCButton.toggleButton.Value = showGUI; }
            }

        }

        void DummyVoid() { }

        void InitButtons()
        {
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
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

                buttonNeedsInit = false;
            }

        }

        void DestroyButtons()
        {
            Debug.Log("Destroying Buttons");
            GameEvents.onGUIApplicationLauncherReady.Remove(OnAppButtonReady);
            GameEvents.onShowUI.Remove(ShowGUI);
            GameEvents.onHideUI.Remove(HideGUI);

            if (CCButton != null)
                ApplicationLauncher.Instance.RemoveModApplication(CCButton);

            showGUI = false;
            buttonNeedsInit = true;
        }

        void OnDestroy()
        {
            Debug.Log("OnDestroy");
            DestroyButtons();
        }
        internal void myLoad()
        {
            Debug.Log("Loading");

            //List<Type> blockedTypes = HeadMaster.blockedTypes;
            //Dictionary<Type, TypePreference> typeMap = HeadMaster.typeMap;
            //Dictionary<TypePreference, List<String>> typePrefDict = HeadMaster.typePrefDict;
            try
            {
                int progress = -1;
                Type loadingType = typeMap.Keys.ElementAt(0);
                //Debug.Log(loadingType);
                String line = null;
                //Debug.Log(Type.GetType(loadingType.ToString(), true, true));
                blockedTypes = new List<Type>();
                TypePreference tp = TypePreference.getDefaultTypePreference();
                using (StreamReader file = new StreamReader(KSPUtil.ApplicationRootPath + "/GameData/ContractFilter/settings.cfg"))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        //Debug.Log(line);


                        //Debug.Log("Reading: " + line);
                        if (line.StartsWith("Blocked: true"))
                        {
                            Debug.Log("BlockedType: " + typeMap.Keys.ElementAt(progress));
                            blockedTypes.Add(typeMap.Keys.ElementAt(progress));
                        }

                        if (line.StartsWith("*"))
                        {
                            progress++;
                            //Debug.Log(typeMap.Keys.ElementAt(progress).Name);
                            //Debug.Log(line.Substring(2, line.Length-1));

                            //Type.GetType(,)
                            loadingType = typeMap.Keys.ElementAt(progress);
                            tp = typeMap[loadingType];
                        }
                        if (line.StartsWith("minFA"))
                        {

                            tp.minFundsAdvance = float.Parse(line.Substring(7));
                            tp.minFundsAdvanceString = line.Substring(7);
                        }
                        else if (line.StartsWith("maxFA"))
                        {
                            tp.maxFundsAdvance = float.Parse(line.Substring(7));
                            tp.maxFundsAdvanceString = line.Substring(7);
                        }
                        else if (line.StartsWith("minFC"))
                        {
                            tp.minFundsCompletion = float.Parse(line.Substring(7));
                            tp.minFundsCompleteString = line.Substring(7);
                        }
                        else if (line.StartsWith("maxFC"))
                        {
                            tp.maxFundsCompletion = float.Parse(line.Substring(7));
                            tp.maxFundsCompleteString = line.Substring(7);
                        }
                        else if (line.StartsWith("minFF"))
                        {
                            tp.minFundsFailure = float.Parse(line.Substring(7));
                            tp.minFundsFailureString = line.Substring(7);
                        }
                        else if (line.StartsWith("maxFF"))
                        {
                            tp.maxFundsFailure = float.Parse(line.Substring(7));
                            tp.maxFundsFailureString = line.Substring(7);
                        }
                        else if (line.StartsWith("minRC"))
                        {
                            tp.minRepCompletion = float.Parse(line.Substring(7));
                            tp.minRepCompleteString = line.Substring(7);
                        }
                        else if (line.StartsWith("maxRC"))
                        {
                            tp.maxRepCompletion = float.Parse(line.Substring(7));
                            tp.maxRepCompleteString = line.Substring(7);
                        }
                        else if (line.StartsWith("minRF"))
                        {
                            tp.minRepFailure = float.Parse(line.Substring(7));
                            tp.minRepFailureString = line.Substring(7);
                        }
                        else if (line.StartsWith("maxRF"))
                        {
                            tp.maxRepFailure = float.Parse(line.Substring(7));
                            tp.maxRepFailureString = line.Substring(7);
                        }
                        else if (line.StartsWith("minSC"))
                        {
                            tp.minScienceCompletion = float.Parse(line.Substring(7));
                            tp.minScienceCompleteString = line.Substring(7);
                        }
                        else if (line.StartsWith("maxSC"))
                        {
                            tp.maxScienceCompletion = float.Parse(line.Substring(7));
                            tp.maxScienceCompleteString = line.Substring(7);
                        }
                        else if (line.StartsWith("minPar"))
                        {
                            tp.minParams = int.Parse(line.Substring(8));
                            tp.minParamsString = line.Substring(8);
                        }
                        else if (line.StartsWith("maxPar"))
                        {
                            tp.maxParams = int.Parse(line.Substring(8));
                            tp.maxParamsString = line.Substring(8);
                        }
                        else if (line.StartsWith("minPrstge"))
                        {
                            tp.minPrestiege = int.Parse(line.Substring(11));
                            tp.minPrestiegeString = line.Substring(11);
                        }
                        else if (line.StartsWith("maxPrstge"))
                        {
                            tp.maxPrestiege = int.Parse(line.Substring(11));
                            tp.maxPrestiegeString = line.Substring(11);

                        }
                        else if (line.StartsWith("`"))
                        { //black body
                            if(!tp.blockedBodies.Contains(line.Substring(1)))
                            {
                                tp.blockedBodies.Add(line.Substring(1));
                            }
                            
                        }
                        else if (line.StartsWith("!"))
                        {
                            if (!tp.autoBodies.Contains(line.Substring(1)))
                            {
                                tp.autoBodies.Add(line.Substring(1));
                            }
                            
                        }
                        else if (line.StartsWith("@"))
                        {
                            if (!tp.blockedStrings.Contains(line.Substring(1)))
                            {
                                tp.blockedStrings.Add(line.Substring(1));
                            }
                            
                        }
                        else if (line.StartsWith("#"))
                        {
                            if (!tp.autoStrings.Contains(line.Substring(1)))
                            {
                                tp.autoStrings.Add(line.Substring(1));
                            }
                            
                        }
                        typeMap[loadingType] = tp;
                    }
                    Debug.Log("Loaded");
                    file.Close();
                    file.Dispose();
                    statusString = "Loaded!";
                    
                }
            }
            catch (Exception e)
            {
                statusString = "Load Failed!";
            }


        }
        internal void mySave()
        {
            Debug.Log("Saving");


            //could prefix type with char and save as currentLoadingType
            //
            try
            {
                if (blockedTypes == null)
                {
                    Debug.Log("blockedTypes is null");
                }
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(KSPUtil.ApplicationRootPath + "/GameData/ContractFilter/settings.cfg", false))
                {
                    Debug.Log("Writing");
                    //Debug.Log(blockedTypes.Count);
                    file.WriteLine("Blocked Types:");
                    if (blockedTypes != null && blockedTypes.Count > 0)
                    {
                        foreach (Type t in blockedTypes)
                        {
                            file.WriteLine("~" + t.Name);
                        }
                    }

                    file.WriteLine("---");
                    //TypePreferences
                    file.WriteLine("Type Preferences: ");
                    int counter = 0;
                    foreach (Type t in typeMap.Keys)
                    {
                        file.WriteLine("*" + t.Name);
                        ///*
                        if (blockedTypes.Contains(t))
                        {
                            file.WriteLine("Blocked: true");
                        }
                        else
                        {
                            file.WriteLine("Blocked: false");
                        }
                        /*
                        foreach (String s in typePrefDict[typeMap[t]])
                        {
                            file.WriteLine(s);
                        }
                        */
                        ///*
                        TypePreference tp = typeMap[t];
                        file.WriteLine("minFA: " + tp.minFundsAdvance);
                        file.WriteLine("maxFA: " + tp.maxFundsAdvance);
                        file.WriteLine("minFC: " + tp.minFundsCompletion);
                        file.WriteLine("maxFC: " + tp.maxFundsCompletion);
                        file.WriteLine("minFF: " + tp.minFundsFailure);
                        file.WriteLine("maxFF: " + tp.maxFundsFailure);
                        file.WriteLine("minRC: " + tp.minRepCompletion);
                        file.WriteLine("maxRC: " + tp.maxRepCompletion);
                        file.WriteLine("minRF: " + tp.minRepFailure);
                        file.WriteLine("maxRF: " + tp.maxRepFailure);
                        file.WriteLine("minSC: " + tp.minScienceCompletion);
                        file.WriteLine("maxSC: " + tp.maxScienceCompletion);
                        file.WriteLine("minPar: " + tp.minParams);
                        file.WriteLine("maxPar: " + tp.maxParams);
                        file.WriteLine("minPrstge: " + tp.minPrestiege);
                        file.WriteLine("maxPrstge: " + tp.maxPrestiege);
                        //*/
                        //bodies
                        file.WriteLine("Blacklisted Bodies:");
                        foreach (String s in tp.blockedBodies)
                        {
                            file.WriteLine("`" + s);
                        }
                        file.WriteLine("Whitelisted Bodies:");
                        foreach (String s in tp.autoBodies)
                        {
                            file.WriteLine("!" + s);
                        }
                        file.WriteLine("Blacklisted Strings");
                        foreach (String s in tp.blockedStrings)
                        {
                            file.WriteLine("@" + s);
                        }
                        file.WriteLine("Whitelisted Strings");
                        foreach (String s in tp.autoBodies)
                        {
                            file.WriteLine("#" + s);
                        }
                    }


                    file.WriteLine("---");
                    statusString = "Saved!";
                    Debug.Log("Saved");
                    file.Close();
                }
            }
            catch (Exception e)
            {
                statusString = "Save failed!";
            }

        }
        void OnGUI()
        {
            if (showGUI)
            {
                if (ContractSystem.Instance.isActiveAndEnabled && ContractSystem.Instance != null)
                {
                    mainGUI = GUI.Window(mainGUID, mainGUI, mainGUIWindow, "Contract Filter~");
                    if (showtypeprefeditor)
                    {
                        Rect rect = new Rect(mainGUI.right, mainGUI.top, 250, 485);
                        rect = GUI.Window(prefEditGUID, rect, editTypePrefGUI, "" + editingType.Name + "~");

                        if (showParameterChangeGUI)
                        {
                            //showParameterChangeGUI = false;
                            showBodyBlacklistGUI = false;
                            showBodyAutoListGUI = false;
                            showAutoParamGUI = false;
                            showParameterListGUI = false;
                            showStringBlacklistGUI = false;
                            showStringWhitelistGUI = false;
                            showEditAllGUI = false;
                            Rect rect1 = new Rect(rect.right, rect.top, 250, 300);
                            rect1 = GUI.Window(parameterGuid, rect1, changeParameterGUI, "Parameter List~");
                            Rect rect2 = new Rect(rect.right, rect.top + rect.height - 185, 250, 300);
                            rect2 = GUI.Window(parameterListGuid, rect2, parameterListGUI, "Blacklisted Parameters~");

                        }
                        if (showAutoParamGUI)
                        {
                            showParameterChangeGUI = false;
                            showBodyBlacklistGUI = false;
                            showBodyAutoListGUI = false;
                            //showAutoParamGUI = false;
                            showParameterListGUI = false;
                            showStringBlacklistGUI = false;
                            showStringWhitelistGUI = false;
                            showEditAllGUI = false;
                            Rect rect1 = new Rect(rect.right, rect.top, 250, 300);
                            rect1 = GUI.Window(autoParameterGuid, rect1, autoParameterListGUI, "Auto-Parameter List~");
                            Rect rect2 = new Rect(rect.right, rect.top + rect.height - 185, 250, 300);
                            rect2 = GUI.Window(autoParameterListGuid, rect2, autoListParameterGUI, "AutoParameters~");
                        }
                        if (showBodyBlacklistGUI)
                        {
                            showParameterChangeGUI = false;
                            //showBodyBlacklistGUI = false;
                            showBodyAutoListGUI = false;
                            showAutoParamGUI = false;
                            showParameterListGUI = false;
                            showStringBlacklistGUI = false;
                            showStringWhitelistGUI = false;
                            showEditAllGUI = false;
                            Rect rect1 = new Rect(rect.right, rect.top, 250, 300);
                            rect1 = GUI.Window(bodyGuid, rect1, bodyBlacklistGUI, "Body List~");
                            Rect rect2 = new Rect(rect.right, rect.top + rect.height - 185, 250, 300);
                            rect2 = GUI.Window(bodyListGuid, rect2, userBodyBlackListGUI, "Blacklisted Bodies~");
                        }
                        if (showBodyAutoListGUI)
                        {
                            showParameterChangeGUI = false;
                            showBodyBlacklistGUI = false;
                            //showBodyAutoListGUI = false;
                            showAutoParamGUI = false;
                            showParameterListGUI = false;
                            showStringBlacklistGUI = false;
                            showStringWhitelistGUI = false;
                            showEditAllGUI = false;
                            Rect rect1 = new Rect(rect.right, rect.top, 250, 300);
                            rect1 = GUI.Window(autoBodyGuid, rect1, autoBodyListGUI, "Body List~");
                            Rect rect2 = new Rect(rect.right, rect.top + rect.height - 185, 250, 300);
                            rect2 = GUI.Window(autoBodyListGuid, rect2, userAutoBodyListGUI, "AutoListed Bodies~");
                        }
                        if (showStringBlacklistGUI)
                        {
                            showParameterChangeGUI = false;
                            showBodyBlacklistGUI = false;
                            showBodyAutoListGUI = false;
                            showAutoParamGUI = false;
                            showParameterListGUI = false;
                            //showStringBlacklistGUI = false;
                            showStringWhitelistGUI = false;
                            showEditAllGUI = false;
                            Rect rect1 = new Rect(rect.right, rect.top, 250, 300);
                            rect1 = GUI.Window(stringGuid, rect1, stringBlackListGUI, "Blacklisted Strings~");
                            //Rect rect2 = new Rect(rect.right, rect.top + rect.height - 180, 250, 300);
                            //rect2 = GUI.Window(stringListGuid, rect2, userStringBlackListGUI, "Blacklisted Strings~");
                        }
                        if (showStringWhitelistGUI)
                        {
                            showParameterChangeGUI = false;
                            showStringBlacklistGUI = false;
                            showBodyAutoListGUI = false;
                            showBodyBlacklistGUI = false;
                            showAutoParamGUI = false;
                            showParameterListGUI = false;
                            showEditAllGUI = false;
                            //showStringWhitelistGUI = false;
                            Rect rect1 = new Rect(rect.right, rect.top, 250, 300);
                            rect1 = GUI.Window(autoStringGuid, rect1, stringAutoListGUI, "Whitelisted Strings~");
                            //Rect rect2 = new Rect(rect.right, rect.top + rect.height - 180, 250, 300);
                            //rect2 = GUI.Window(autoStringListGuid, rect2, userStringAutoListGUI, "AutoListed Strings~");
                        }

                    }
                    if (showEditAllGUI)
                    {
                        showParameterChangeGUI = false;
                        showStringBlacklistGUI = false;
                        showBodyAutoListGUI = false;
                        showBodyBlacklistGUI = false;
                        showAutoParamGUI = false;
                        showParameterListGUI = false;
                        showStringWhitelistGUI = false;

                        Rect rect1 = new Rect(mainGUI.right, mainGUI.top, 250, 460);
                        rect1 = GUI.Window(editAllGuid, rect1, editAllWindow, "Edit All~");

                        if (showAllParameterChangeGUI)
                        {
                            //showParameterChangeGUI = false;
                            showAllBodyBlacklistGUI = false;
                            showAllBodyAutoListGUI = false;
                            showAllAutoParamGUI = false;
                            showAllParameterListGUI = false;
                            showAllStringBlacklistGUI = false;
                            showAllStringWhitelistGUI = false;;
                            Rect rect2 = new Rect(rect1.right, rect1.top, 250, 300);
                            rect2 = GUI.Window(allparameterGuid, rect2, allParamWindow, "Parameter List~");
                            Rect rect3 = new Rect(rect1.right, rect1.top + rect1.height - 160, 250, 300);
                            rect3 = GUI.Window(allparameterListGuid, rect3, allParamListWindow, "Blacklisted Parameters~");

                        }
                        if (showAllAutoParamGUI)
                        {
                            showAllParameterChangeGUI = false;
                            showAllBodyBlacklistGUI = false;
                            showAllBodyAutoListGUI = false;
                            //showAutoParamGUI = false;
                            showAllParameterListGUI = false;
                            showAllStringBlacklistGUI = false;
                            showAllStringWhitelistGUI = false;
                            Rect rect2 = new Rect(rect1.right, rect1.top, 250, 300);
                            rect2 = GUI.Window(allautoParameterGuid, rect2, allAutoParameterListGUI, "Auto-Parameter List~");
                            Rect rect3 = new Rect(rect1.right, rect1.top + rect1.height - 160, 250, 300);
                            rect3 = GUI.Window(allautoParameterListGuid, rect3, allAutoListParameterGUI, "AutoParameters~");
                        }
                        if (showAllBodyBlacklistGUI)
                        {
                            showAllParameterChangeGUI = false;
                            //showBodyBlacklistGUI = false;
                            showAllBodyAutoListGUI = false;
                            showAllAutoParamGUI = false;
                            showAllParameterListGUI = false;
                            showAllStringBlacklistGUI = false;
                            showAllStringWhitelistGUI = false;
                            Rect rect2 = new Rect(rect1.right, rect1.top, 250, 300);
                            rect2 = GUI.Window(allbodyGuid, rect2, allBodyBlacklistGUI, "Body List~");
                            Rect rect3 = new Rect(rect1.right, rect1.top + rect1.height - 160, 250, 300);
                            rect3 = GUI.Window(allbodyListGuid, rect3, allUserBodyBlackListGUI, "Blacklisted Bodies~");
                        }
                        if (showAllBodyAutoListGUI)
                        {
                            showAllParameterChangeGUI = false;
                            showAllBodyBlacklistGUI = false;
                            //showBodyAutoListGUI = false;
                            showAllAutoParamGUI = false;
                            showAllParameterListGUI = false;
                            showAllStringBlacklistGUI = false;
                            showAllStringWhitelistGUI = false;
                            Rect rect2 = new Rect(rect1.right, rect1.top, 250, 300);
                            rect2 = GUI.Window(allautoBodyGuid, rect2, allAutoBodyListGUI, "Body List~");
                            Rect rect3 = new Rect(rect1.right, rect1.top + rect1.height - 160, 250, 300);
                            rect3 = GUI.Window(allautoBodyListGuid, rect3, allUserAutoBodyListGUI, "AutoListed Bodies~");
                        }
                        if (showAllStringBlacklistGUI)
                        {
                            showAllParameterChangeGUI = false;
                            showAllBodyBlacklistGUI = false;
                            showAllBodyAutoListGUI = false;
                            showAllAutoParamGUI = false;
                            showAllParameterListGUI = false;
                            //showStringBlacklistGUI = false;
                            showAllStringWhitelistGUI = false;
                            Rect rect2 = new Rect(rect1.right, rect1.top, 250, 300);
                            rect2 = GUI.Window(allstringGuid, rect2, allStringBlackListGUI, "Blacklisted Strings~");
                            //Rect rect2 = new Rect(rect.right, rect.top + rect.height - 180, 250, 300);
                            //rect2 = GUI.Window(stringListGuid, rect2, userStringBlackListGUI, "Blacklisted Strings~");
                        }
                        if (showAllStringWhitelistGUI)
                        {
                            showAllParameterChangeGUI = false;
                            showAllStringBlacklistGUI = false;
                            showAllBodyAutoListGUI = false;
                            showAllBodyBlacklistGUI = false;
                            showAllAutoParamGUI = false;
                            showAllParameterListGUI = false;
                            //showStringWhitelistGUI = false;
                            Rect rect2 = new Rect(rect1.right, rect1.top, 250, 300);
                            rect2 = GUI.Window(allautoStringGuid, rect2, allStringWhiteListGUI, "Whitelisted Strings~");
                            //Rect rect2 = new Rect(rect.right, rect.top + rect.height - 180, 250, 300);
                            //rect2 = GUI.Window(autoStringListGuid, rect2, userStringAutoListGUI, "AutoListed Strings~");
                        }
                    }
                }
            }



        }
        public void editAllWindow(int windowID)
        {
            GUI.backgroundColor = new Color(0,0,0);
            if(ContractSystem.Instance.isActiveAndEnabled && ContractSystem.Instance != null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("");
                scrollPosition11 = GUILayout.BeginScrollView(scrollPosition11, GUILayout.Width(235), GUILayout.Height(250));
                //mainGUI.position = scrollPosition;
                //Debug.Log("2");
                GUILayout.BeginHorizontal();
                GUILayout.Label("minFundsAdvance:");
                //minFundsAdvance = tp.minFundsAdvance.ToString();
                minFundsAdvanceString = GUILayout.TextField(minFundsAdvanceString);
                //tp.minFundsAdvance = float.Parse(minFundsAdvance);
                //typePrefDict[tp][0] = GUILayout.TextField(typePrefDict[tp][0]); //this fucks up
                GUILayout.EndHorizontal();
                //Debug.Log("3");
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxFundsAdvance:");
                maxFundsAdvanceString = GUILayout.TextField(maxFundsAdvanceString);
                //tp.maxFundsAdvance = float.Parse(GUILayout.TextField(tp.maxFundsAdvance.ToString()));
                GUILayout.EndHorizontal(); ;
                GUILayout.BeginHorizontal();
                GUILayout.Label("minFundsComplete:");
                minFundsCompleteString = GUILayout.TextField(minFundsCompleteString);
                //tp.minFundsCompletion = float.Parse(GUILayout.TextField(tp.minFundsCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxFundsComplete:");
                maxFundsCompleteString = GUILayout.TextField(maxFundsCompleteString);
                //tp.maxFundsCompletion = float.Parse(GUILayout.TextField(tp.maxFundsCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minFundsFailure:");
                minFundsFailureString = GUILayout.TextField(minFundsFailureString);
                //tp.minFundsFailure = float.Parse(GUILayout.TextField(tp.minFundsFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxFundsFailure:");
                maxFundsFailureString = GUILayout.TextField(maxFundsFailureString);
                //tp.maxFundsFailure = float.Parse(GUILayout.TextField(tp.maxFundsFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minRepComplete:");
                minRepCompleteString = GUILayout.TextField(minRepCompleteString);
                //tp.minRepCompletion = float.Parse(GUILayout.TextField(tp.minRepCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxRepComplete:");
                maxRepCompleteString = GUILayout.TextField(maxRepCompleteString);
                //tp.maxRepCompletion = float.Parse(GUILayout.TextField(tp.maxRepCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minRepFailure:");
                minRepFailureString = GUILayout.TextField(minRepFailureString);
                //tp.minRepFailure = float.Parse(GUILayout.TextField(tp.minRepFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxRepFailure:");
                maxRepFailureString = GUILayout.TextField(maxRepFailureString);
                //tp.maxRepFailure = float.Parse(GUILayout.TextField(tp.maxRepFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minScienceComplete:");
                minScienceCompleteString = GUILayout.TextField(minScienceCompleteString);
                //tp.minScienceCompletion = float.Parse(GUILayout.TextField(tp.minScienceCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxScienceComplete:");
                maxScienceCompleteString = GUILayout.TextField(maxScienceCompleteString);
                //tp.maxScienceCompletion = float.Parse(GUILayout.TextField(tp.maxScienceCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minParameters:");
                minParamsString = GUILayout.TextField(minParamsString);
                //tp.minParams = int.Parse(GUILayout.TextField(tp.minParams.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxParameters:");
                maxParamsString = GUILayout.TextField(maxParamsString);
                //tp.maxParams = int.Parse(GUILayout.TextField(tp.maxParams.ToString()));
                GUILayout.EndHorizontal();

                //Note: have in-scope strings which convert to float/int, as the above doesn't work
                GUILayout.EndScrollView();

                if (GUILayout.Button("Blacklist Parameters"))
                {
                    showAllParameterChangeGUI = !showAllParameterChangeGUI;
                    showAllParameterListGUI = !showAllParameterListGUI;
                }
                if (GUILayout.Button("AutoAccept Parameters"))
                {
                    showAllAutoParamGUI = !showAllAutoParamGUI;

                }
                if (GUILayout.Button("Blacklist Bodies"))
                {
                    showAllBodyBlacklistGUI = !showAllBodyBlacklistGUI;
                }
                if (GUILayout.Button("AutoAccept Bodies"))
                {
                    showAllBodyAutoListGUI = !showAllBodyAutoListGUI;
                }
                if (GUILayout.Button("Blacklist Strings"))
                {
                    showAllStringBlacklistGUI = !showAllStringBlacklistGUI;
                }
                if (GUILayout.Button("Whitelist Strings"))
                {
                    showAllStringWhitelistGUI = !showAllStringWhitelistGUI;
                }
                if(GUILayout.Button("Close"))
                {
                    statusString = "Saving All...";
                    //mySave();
                    foreach (Type t in Contracts.ContractSystem.ContractTypes)
                    {
                        try
                        {
                            //Debug.Log(typePrefDict.Count);
                            //Debug.Log(typePrefDict[typeMap[editingType]].Count);
                            ///*
                            TypePreference tp1 = typeMap[t];
                            statusString = "Saving: " + t.Name;
                            tp1.minFundsAdvance = float.Parse(minFundsAdvanceString);
                            //Debug.Log("saving: " + tp.minFundsAdvance);
                            tp1.maxFundsAdvance = float.Parse(maxFundsAdvanceString);
                            tp1.minFundsCompletion = float.Parse(minFundsCompleteString);
                            tp1.maxFundsCompletion = float.Parse(maxFundsCompleteString);
                            tp1.minFundsFailure = float.Parse(minFundsFailureString);
                            tp1.maxFundsFailure = float.Parse(maxFundsFailureString);

                            tp1.minRepCompletion = float.Parse(minRepCompleteString);
                            tp1.maxRepCompletion = float.Parse(maxRepCompleteString);
                            tp1.minRepFailure = float.Parse(minRepFailureString);
                            tp1.maxRepFailure = float.Parse(maxRepFailureString);

                            tp1.minScienceCompletion = float.Parse(minScienceCompleteString);
                            tp1.maxScienceCompletion = float.Parse(maxScienceCompleteString);

                            tp1.minParams = int.Parse(minParamsString);
                            tp1.maxParams = int.Parse(maxParamsString);
                            tp1.minPrestiege = int.Parse(minPrestiegeString);
                            tp1.maxPrestiege = int.Parse(maxPrestiegeString);

                            

                            tp1.autoParameters = whiteParams;
                            tp1.blockedParameters = blockedParams;
                            tp1.autoBodies = whiteBodies;
                            tp1.blockedBodies = blackBodies;
                            tp1.autoStrings = whiteStrings;
                            tp1.blockedStrings = blackStrings;

                            typeMap[t] = tp1;
                            //tp = tp1;
                            //*/
                            //Debug.Log(tp.minFundsCompletion);
                            statusString = "Saved!";
                            
                        }
                        catch (Exception e)
                        {
                            statusString = "Failed to save!";
                        }
                    }
                    stringthing = "";
                    stringthing1 = "";
                    Debug.Log("Settings Saved");
                    statusString = "Saved!";
                    //idleWatch.Restart();
                    idleWatch.Reset();
                    idleWatch.Start();

                    showEditAllGUI = false;
                }
            }
        }
        public void allParamWindow(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            List<Type> bloop = blockedParams;
            scrollPosition13 = GUILayout.BeginScrollView(scrollPosition13, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in ContractSystem.ParameterTypes)
            {
                if (!bloop.Contains(cp))
                {
                    if (GUILayout.Button("" + cp.Name))
                    {
                        blockedParams.Add(cp);

                    }
                }
            }
            GUILayout.EndScrollView();
        }
        public void allParamListWindow(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            List<Type> bloop = blockedParams;

            scrollPosition14 = GUILayout.BeginScrollView(scrollPosition14, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in bloop)
            {
                if (GUILayout.Button(cp.Name))
                {
                    try
                    {
                        blockedParams.Remove(cp);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();
        }

        public void allAutoParameterListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            List<Type> bloop = whiteParams;
            scrollPosition13 = GUILayout.BeginScrollView(scrollPosition13, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in ContractSystem.ParameterTypes)
            {
                if (!bloop.Contains(cp))
                {
                    if (GUILayout.Button("" + cp.Name))
                    {
                        if (blockedParams.Contains(cp))
                        {
                            try
                            {
                                blockedParams.Remove(cp);
                            }
                            catch (Exception e)
                            {

                            }

                        }
                        whiteParams.Add(cp);

                    }
                }
            }
            GUILayout.EndScrollView();
        }

        public void allAutoListParameterGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            List<Type> bloop = new List<Type>();
            bloop = whiteParams;

            scrollPosition14 = GUILayout.BeginScrollView(scrollPosition14, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in bloop)
            {
                if (GUILayout.Button(cp.Name))
                {
                    try
                    {
                        whiteParams.Remove(cp);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();
        }
        public void allBodyBlacklistGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            scrollPosition15 = GUILayout.BeginScrollView(scrollPosition15, GUILayout.Width(235), GUILayout.Height(290));
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!blackBodies.Contains(body.name))
                {
                    if (GUILayout.Button(body.name))
                    {
                        blackBodies.Add(body.name);
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        public void allUserBodyBlackListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            scrollPosition16 = GUILayout.BeginScrollView(scrollPosition16, GUILayout.Width(235), GUILayout.Height(290));
            List<String> list = blackBodies;
            foreach (String s in list)
            {
                if (GUILayout.Button(s))
                {
                    try
                    {
                        blackBodies.Remove(s);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();
        }
        public void allAutoBodyListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            scrollPosition17 = GUILayout.BeginScrollView(scrollPosition17, GUILayout.Width(235), GUILayout.Height(290));
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!whiteBodies.Contains(body.name))
                {
                    if (GUILayout.Button(body.name))
                    {
                        whiteBodies.Add(body.name);
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        public void allUserAutoBodyListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            scrollPosition18 = GUILayout.BeginScrollView(scrollPosition18, GUILayout.Width(235), GUILayout.Height(290));
            List<String> list = whiteBodies;
            foreach (String s in list)
            {
                if (GUILayout.Button(s))
                {
                    try
                    {
                        whiteBodies.Remove(s);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();
        }
        public void allStringBlackListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            GUILayout.Label("Note: The strings are case sensitive");
            stringthing = GUILayout.TextField(stringthing);
            if (GUILayout.Button("Add"))
            {
                if (!blackStrings.Contains(stringthing))
                {
                    blackStrings.Add(stringthing);
                }
            }
            scrollPosition19 = GUILayout.BeginScrollView(scrollPosition19, GUILayout.Width(235), GUILayout.Height(290));
            foreach (String s in blackStrings)
            {
                if (GUILayout.Button(s))
                {
                    blackStrings.Remove(s);
                }
            }
            GUILayout.EndScrollView();
        }
        public void allStringWhiteListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            GUILayout.Label("Note: The strings are case sensitive");
            stringthing1 = GUILayout.TextField(stringthing1);
            if (GUILayout.Button("Add"))
            {
                if (!whiteStrings.Contains(stringthing1))
                {
                    whiteStrings.Add(stringthing1);
                }
            }
            scrollPosition20 = GUILayout.BeginScrollView(scrollPosition20, GUILayout.Width(235), GUILayout.Height(290));
            foreach (String s in whiteStrings)
            {
                if (GUILayout.Button(s))
                {
                    whiteStrings.Remove(s);
                }
            }
            GUILayout.EndScrollView();
        }
        public void mainGUIWindow(int windowID)
        {
            if (ContractSystem.Instance.isActiveAndEnabled && ContractSystem.Instance != null)
            {
                //float value = 100;
                //value = GUILayout.VerticalScrollbar(value, 5, 100, 0);
                GUILayout.Label("Choose the contract type you want to edit:");
                scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(235), GUILayout.Height(375));
                //mainGUI.position = scrollPosition;

                foreach (Type t in Contracts.ContractSystem.ContractTypes)
                {
                    if (blockedTypes.Contains(t))
                    {
                        GUI.backgroundColor = new Color(1.0f, 0.25f, 0.25f);
                        if (GUILayout.Button(t.Name))
                        {
                            editingType = t;
                            showtypeprefeditor = true;
                            showParameterChangeGUI = false;
                            showParameterListGUI = false;
                            showAutoParamGUI = false;
                            showEditAllGUI = false;
                        }
                        GUI.backgroundColor = new Color(0.0f, 0.0f, 0f);
                    }
                    else
                    {
                        GUI.backgroundColor = new Color(0.25f, 1.0f, 0.25f);
                        if (GUILayout.Button(t.Name))
                        {
                            editingType = t;
                            showtypeprefeditor = true;
                            showParameterChangeGUI = false;
                            showParameterListGUI = false;
                            showAutoParamGUI = false;
                            showEditAllGUI = false;
                        }
                        GUI.backgroundColor = new Color(0.0f, 0.0f, 0f);
                    }


                }
                GUILayout.EndScrollView();
                if (GUILayout.Button("Edit All"))
                {
                    showEditAllGUI = !showEditAllGUI;
                    showtypeprefeditor = false;
                    showParameterChangeGUI = false;
                    showParameterListGUI = false;
                    showAutoParamGUI = false;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Time interval:");
                sortTimeString = GUILayout.TextField(sortTimeString + " s");
                sortTime = float.Parse(sortTimeString);
                GUILayout.EndHorizontal();

                if (isSorting)
                {
                    if (GUILayout.Button("Stop Sorting"))
                    {
                        statusString = "Stopped sorting!";
                        //idleWatch.Restart();
                        idleWatch.Reset();
                        idleWatch.Start();
                        isSorting = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("Start Sorting"))
                    {
                        statusString = "Sorting!";
                        //idleWatch.Restart();
                        idleWatch.Reset();
                        idleWatch.Start();
                        isSorting = true;

                    }
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save"))
                {
                    statusString = "Saving...";
                    mySave();
                    statusString = "Saved!";
                    //idleWatch.Restart();
                    idleWatch.Reset();
                    idleWatch.Start();
                }
                if (GUILayout.Button("Load"))
                {
                    statusString = "Loading...";
                    myLoad();
                    statusString = "Loaded!";
                    //idleWatch.Restart();
                    idleWatch.Reset();
                    idleWatch.Start();
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Status:");
                GUILayout.Label(statusString);
                if (GUILayout.Button("Close"))
                {
                    //showGUI = false;
                    OnAppLaunchToggleOff();
                }
            }

            GUI.DragWindow();
        }

        public void editTypePrefGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            if (ContractSystem.Instance.isActiveAndEnabled && ContractSystem.Instance != null)
            {
                //minFunds

                //Debug.Log(editingType);

                TypePreference tp = typeMap[editingType];

                //maxFunds
                GUILayout.BeginVertical();
                scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, GUILayout.Width(235), GUILayout.Height(250));
                //mainGUI.position = scrollPosition;
                //Debug.Log("2");
                GUILayout.BeginHorizontal();
                GUILayout.Label("minFundsAdvance:");
                //minFundsAdvance = tp.minFundsAdvance.ToString();
                tp.minFundsAdvanceString = GUILayout.TextField(tp.minFundsAdvanceString);
                //tp.minFundsAdvance = float.Parse(minFundsAdvance);
                //typePrefDict[tp][0] = GUILayout.TextField(typePrefDict[tp][0]); //this fucks up
                GUILayout.EndHorizontal();
                //Debug.Log("3");
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxFundsAdvance:");
                tp.maxFundsAdvanceString = GUILayout.TextField(tp.maxFundsAdvanceString);
                //tp.maxFundsAdvance = float.Parse(GUILayout.TextField(tp.maxFundsAdvance.ToString()));
                GUILayout.EndHorizontal(); ;
                GUILayout.BeginHorizontal();
                GUILayout.Label("minFundsComplete:");
                tp.minFundsCompleteString = GUILayout.TextField(tp.minFundsCompleteString);
                //tp.minFundsCompletion = float.Parse(GUILayout.TextField(tp.minFundsCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxFundsComplete:");
                tp.maxFundsCompleteString = GUILayout.TextField(tp.maxFundsCompleteString);
                //tp.maxFundsCompletion = float.Parse(GUILayout.TextField(tp.maxFundsCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minFundsFailure:");
                tp.minFundsFailureString = GUILayout.TextField(tp.minFundsFailureString);
                //tp.minFundsFailure = float.Parse(GUILayout.TextField(tp.minFundsFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxFundsFailure:");
                tp.maxFundsFailureString = GUILayout.TextField(tp.maxFundsFailureString);
                //tp.maxFundsFailure = float.Parse(GUILayout.TextField(tp.maxFundsFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minRepComplete:");
                tp.minRepCompleteString = GUILayout.TextField(tp.minRepCompleteString);
                //tp.minRepCompletion = float.Parse(GUILayout.TextField(tp.minRepCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxRepComplete:");
                tp.maxRepCompleteString = GUILayout.TextField(tp.maxRepCompleteString);
                //tp.maxRepCompletion = float.Parse(GUILayout.TextField(tp.maxRepCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minRepFailure:");
                tp.minRepFailureString = GUILayout.TextField(tp.minRepFailureString);
                //tp.minRepFailure = float.Parse(GUILayout.TextField(tp.minRepFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxRepFailure:");
                tp.maxRepFailureString = GUILayout.TextField(tp.maxRepFailureString);
                //tp.maxRepFailure = float.Parse(GUILayout.TextField(tp.maxRepFailure.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minScienceComplete:");
                tp.minScienceCompleteString = GUILayout.TextField(tp.minScienceCompleteString);
                //tp.minScienceCompletion = float.Parse(GUILayout.TextField(tp.minScienceCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxScienceComplete:");
                tp.maxScienceCompleteString = GUILayout.TextField(tp.maxScienceCompleteString);
                //tp.maxScienceCompletion = float.Parse(GUILayout.TextField(tp.maxScienceCompletion.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("minParameters:");
                tp.minParamsString = GUILayout.TextField(tp.minParamsString);
                //tp.minParams = int.Parse(GUILayout.TextField(tp.minParams.ToString()));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("maxParameters:");
                tp.maxParamsString = GUILayout.TextField(tp.maxParamsString);
                //tp.maxParams = int.Parse(GUILayout.TextField(tp.maxParams.ToString()));
                GUILayout.EndHorizontal();

                //Note: have in-scope strings which convert to float/int, as the above doesn't work
                GUILayout.EndScrollView();

                typeMap[editingType] = tp;
                GUILayout.EndVertical();
                if (blockedTypes.Contains(editingType))
                {

                    if (GUILayout.Button("Accept Type"))
                    {
                        blockedTypes.Remove(editingType);
                    }
                    //GUI.backgroundColor = new Color(1.0f, 1.0f, 1.00f);
                }
                else
                {
                    //GUI.backgroundColor = new Color(1.0f, 0.0f, 0f);
                    if (GUILayout.Button("Reject Type"))
                    {
                        blockedTypes.Add(editingType);
                    }
                    //GUI.backgroundColor = new Color(1.0f, 1.0f, 1.00f);
                }
                if (GUILayout.Button("Blacklist Parameters"))
                {
                    showParameterChangeGUI = !showParameterChangeGUI;
                    showParameterListGUI = !showParameterListGUI;
                }
                if (GUILayout.Button("AutoAccept Parameters"))
                {
                    showAutoParamGUI = !showAutoParamGUI;

                }
                if (GUILayout.Button("Blacklist Bodies"))
                {
                    showBodyBlacklistGUI = !showBodyBlacklistGUI;
                }
                if (GUILayout.Button("AutoAccept Bodies"))
                {
                    showBodyAutoListGUI = !showBodyAutoListGUI;
                }
                if (GUILayout.Button("Blacklist Strings"))
                {
                    showStringBlacklistGUI = !showStringBlacklistGUI;
                }
                if (GUILayout.Button("Whitelist Strings"))
                {
                    showStringWhitelistGUI = !showStringWhitelistGUI;
                }

                if (GUILayout.Button("Close"))
                {
                    statusString = "Saving...";
                    try
                    {
                        //Debug.Log(typePrefDict.Count);
                        //Debug.Log(typePrefDict[typeMap[editingType]].Count);
                        ///*
                        //TypePreference tp1 = typeMap[editingType];
                        tp.minFundsAdvance = float.Parse(tp.minFundsAdvanceString);
                        //Debug.Log("saving: " + tp.minFundsAdvance);
                        tp.maxFundsAdvance = float.Parse(tp.maxFundsAdvanceString);
                        tp.minFundsCompletion = float.Parse(tp.minFundsCompleteString);
                        tp.maxFundsCompletion = float.Parse(tp.maxFundsCompleteString);
                        tp.minFundsFailure = float.Parse(tp.minFundsFailureString);
                        tp.maxFundsFailure = float.Parse(tp.maxFundsFailureString);

                        tp.minRepCompletion = float.Parse(tp.minRepCompleteString);
                        tp.maxRepCompletion = float.Parse(tp.maxRepCompleteString);
                        tp.minRepFailure = float.Parse(tp.minRepFailureString);
                        tp.maxRepFailure = float.Parse(tp.maxRepFailureString);

                        tp.minScienceCompletion = float.Parse(tp.minScienceCompleteString);
                        tp.maxScienceCompletion = float.Parse(tp.maxScienceCompleteString);

                        tp.minParams = int.Parse(tp.minParamsString);
                        tp.maxParams = int.Parse(tp.maxParamsString);
                        tp.minPrestiege = int.Parse(tp.minPrestiegeString);
                        tp.maxPrestiege = int.Parse(tp.maxPrestiegeString);

                        typeMap[editingType] = tp;
                        //tp = tp1;
                        //*/
                        //Debug.Log(tp.minFundsCompletion);
                        statusString = "Saved!";
                        Debug.Log("Settings Saved");
                    }
                    catch (Exception e)
                    {
                        statusString = "Failed to save!";
                    }
                    showtypeprefeditor = false;
                }

            }

        }
        public void changeParameterGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            List<Type> bloop = tp.blockedParameters;
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in ContractSystem.ParameterTypes)
            {
                if (!bloop.Contains(cp))
                {
                    if (GUILayout.Button("" + cp.Name))
                    {
                        tp.blockedParameters.Add(cp);

                    }
                }
            }
            GUILayout.EndScrollView();


        }
        public void parameterListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            List<Type> bloop = tp.blockedParameters;

            scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in bloop)
            {
                if (GUILayout.Button(cp.Name))
                {
                    try
                    {
                        tp.blockedParameters.Remove(cp);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();

        }
        public void autoParameterListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            List<Type> bloop = tp.autoParameters;
            scrollPosition3 = GUILayout.BeginScrollView(scrollPosition3, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in ContractSystem.ParameterTypes)
            {
                if (!bloop.Contains(cp))
                {
                    if (GUILayout.Button("" + cp.Name))
                    {
                        if (tp.blockedParameters.Contains(cp))
                        {
                            try
                            {
                                tp.blockedParameters.Remove(cp);
                            }
                            catch (Exception e)
                            {

                            }

                        }
                        tp.autoParameters.Add(cp);

                    }
                }
            }
            GUILayout.EndScrollView();


        }
        public void autoListParameterGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            List<Type> bloop = new List<Type>();
            bloop = tp.autoParameters;

            scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, GUILayout.Width(235), GUILayout.Height(290));
            foreach (Type cp in bloop)
            {
                if (GUILayout.Button(cp.Name))
                {
                    try
                    {
                        tp.autoParameters.Remove(cp);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();

        }
        public void bodyBlacklistGUI(int windowId)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            scrollPosition5 = GUILayout.BeginScrollView(scrollPosition5, GUILayout.Width(235), GUILayout.Height(290));
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!tp.blockedBodies.Contains(body.name))
                {
                    if (GUILayout.Button(body.name))
                    {
                        tp.blockedBodies.Add(body.name);
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        public void userBodyBlackListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            scrollPosition6 = GUILayout.BeginScrollView(scrollPosition6, GUILayout.Width(235), GUILayout.Height(290));
            TypePreference tp = typeMap[editingType];
            List<String> list = tp.blockedBodies;
            foreach (String s in list)
            {
                if (GUILayout.Button(s))
                {
                    try
                    {
                        tp.blockedBodies.Remove(s);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();
        }
        public void autoBodyListGUI(int windowId)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            scrollPosition7 = GUILayout.BeginScrollView(scrollPosition7, GUILayout.Width(235), GUILayout.Height(290));
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (!tp.autoBodies.Contains(body.name))
                {
                    if (GUILayout.Button(body.name))
                    {
                        tp.autoBodies.Add(body.name);
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        public void userAutoBodyListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            scrollPosition8 = GUILayout.BeginScrollView(scrollPosition8, GUILayout.Width(235), GUILayout.Height(290));
            TypePreference tp = typeMap[editingType];
            List<String> list = tp.autoBodies;
            foreach (String s in list)
            {
                if (GUILayout.Button(s))
                {
                    try
                    {
                        tp.autoBodies.Remove(s);
                    }
                    catch (Exception e)
                    {

                    }

                }
            }
            GUILayout.EndScrollView();
        }
        public void stringBlackListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            GUILayout.Label("Note: The strings are case sensitive");
            stringthing = GUILayout.TextField(stringthing);
            if (GUILayout.Button("Add"))
            {
                if (!tp.blockedStrings.Contains(stringthing))
                {
                    tp.blockedStrings.Add(stringthing);
                }
            }
            scrollPosition9 = GUILayout.BeginScrollView(scrollPosition9, GUILayout.Width(235), GUILayout.Height(290));
            foreach (String s in tp.blockedStrings)
            {
                if (GUILayout.Button(s))
                {
                    tp.blockedStrings.Remove(s);
                }
            }
            GUILayout.EndScrollView();
        }
        public void stringAutoListGUI(int windowID)
        {
            GUI.backgroundColor = new Color(0, 0, 0);
            TypePreference tp = typeMap[editingType];
            GUILayout.Label("Note: The strings are case sensitive");
            stringthing1 = GUILayout.TextField(stringthing1);
            if (GUILayout.Button("Add"))
            {
                if (!tp.autoStrings.Contains(stringthing1))
                {
                    tp.autoStrings.Add(stringthing1);
                }
            }
            scrollPosition10 = GUILayout.BeginScrollView(scrollPosition10, GUILayout.Width(235), GUILayout.Height(290));
            foreach (String s in tp.autoStrings)
            {
                if (GUILayout.Button(s))
                {
                    tp.autoStrings.Remove(s);
                }
            }
            GUILayout.EndScrollView();
        }
        public void initTypePreferences()
        {
            Debug.Log("Initing type preferences");
            //typeMap = new Dictionary<Type, TypePreference>();
            Type CConfigType = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
                .SingleOrDefault(t => t.FullName == "ContractConfigurator.ConfiguredContract");
            Type CConfigType1 = AssemblyLoader.loadedAssemblies.SelectMany(a => a.assembly.GetExportedTypes())
                .SingleOrDefault(t => t.FullName == "ContractConfigurator.ContractType");
            MethodInfo methodInfo = null;
            FieldInfo fI = null;
            if(CConfigType != null)
            {
                methodInfo = CConfigType.GetProperty("contractType").GetGetMethod();
            }
            if(CConfigType1 != null)
            {
                fI = CConfigType1.GetField("name");
            }

            
            
            if (ContractSystem.ContractTypes != null)
            {
                foreach (Type t in ContractSystem.ContractTypes)
                {
                    Debug.Log("Adding Type: " + t.Name);
                    if(t.Name != "ConfiguredContract")
                    {
                        TypePreference tp = TypePreference.getDefaultTypePreference();
                        typeMap.Add(t, tp);
                    }
                    else
                    {
                        Debug.Log(t.Name);
                        object obj = methodInfo.Invoke(t,null);
                        String name = (String)fI.GetValue(obj);
                        Debug.Log("Super special shit name: " + name);
                    }
                    
                }
                foreach(String s in CCstrings)
                {

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
            if (c.ReputationCompletion > tp.maxRepFailure)
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

                if (c.Title.Contains(s))
                {
                    
                    return true;
                }

            }
            if (tp.blockedBodies.Contains("Kerbin"))
            {
                if (checkForString(c, "Launch Site"))
                {
                    
                    return true;
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
            foreach (Type cp in tp.blockedParameters)
            {
                foreach (ContractParameter CP in c.AllParameters.ToList())
                {
                    if (cp.Equals(CP.GetType()))
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
            foreach (String s in tp.autoBodies)
            {
                if (checkForString(c, s))
                {
                    
                    return true;
                }
            }
            if (tp.autoBodies.Contains("Kerbin"))
            {
                if (checkForString(c, "Launch Site"))
                {
                    Debug.Log("Adding: " + c.Title + " to be accepted because of whited body");
                    return true;
                }
            }

            return false;
        }
        public bool checkForWhitedParameter(Contract c)
        {
            Type t = c.GetType();
            TypePreference tp = typeMap[t];
            foreach (Type cp in tp.autoParameters)
            {
                foreach (ContractParameter CP in c.AllParameters.ToList())
                {
                    if (cp.Equals(CP.GetType()))
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
                if (tp.autoPrestieges.Contains(p))
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
                    if (aM.Equals(am))
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
                if (type.Equals(t))
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
                if (c.Agent.Equals(a))
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

                if (c.ContractState == Contract.State.Offered)
                {
                    Debug.Log("Removing: " + c.Title);
                    statusString = "Declining: " + c.Title;
                    c.Decline();
                }

                //Contracts.ContractSystem.Instance.Contracts.Remove(c);
            }
        }
        public void acceptWhitedContracts(List<Contract> contractList)
        {
            foreach (Contract c in contractList)
            {

                //Debug.Log(c.DateAccepted);
                if (c.ContractState == Contract.State.Offered)
                {
                    Debug.Log("Accepting: " + c.Title);
                    statusString = "Accepting: " + c.Title;
                    c.Accept();
                }


                //c.AutoAccept = true;
                //c.AutoAccept = false;
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
