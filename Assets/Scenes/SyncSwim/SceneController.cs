using UnityEngine;
using UnityEngine.UI; // buttons
using UnityEngine.EventSystems; // canvas elements (buttons)
using System.Collections;
using System.Collections.Generic; // lists
using System.IO; // save file
using System.Linq; // list comparisons
using TMPro; // dropdowns

public class SceneController : MonoBehaviour
{
    // dummy index 0
    public bool[] levelComplete = new bool[8];
    public LevelChanger[] levels = new LevelChanger[8];
    public SpriteRenderer[] stars = new SpriteRenderer[8];

    public bool allLevelsComplete;

    // set these variables in the Inspector
    public int levelSelected = 0;
    public SpriteRenderer infoBox;
    public SpriteRenderer diveSpriteRenderer;
    public Animator youAnimator;
    public Animator partnerAnimator;
    public GameObject canvas;
    // canvas panels
    public Transform indicators;
    public Transform verticalTransform;
    public Transform gridTransform;
    // hidden pblock holders
    public Transform blockHolder;
    public Transform pblockHolder;

    // uArray: list of ints, when do "you" flip
    // pArray: array of lists of ints, per level: when does "partner" flip
    private List<int> uArray = new List<int>();
    private List<int>[] pArray = new List<int>[8];
    
    // pblocks: provided blocks per level
    private List<string>[] pblocks = new List<string>[8];

    // line colors
    private string[] lineColors = new string[8] {"","red","blue","blue","black","blue","black","green"};

    // on scene load
    void Start ()
    {
        // read save data -> which levels are completed
        // set level icons
        if (File.Exists("swimData.txt")) 
        {
            // read file -> completed levels into bool array
            bool[] swimData = File.ReadAllLines("swimData.txt").Select(line => bool.Parse(line)).ToArray();

            // check if all levels complete
            allLevelsComplete = (swimData[0] && swimData[1] && swimData[2] && swimData[3] && swimData[4] && swimData[5] && swimData[6]);
            Debug.Log(""+allLevelsComplete);

            // read: file lines 0-6
            // into: levelComplete indices 1-7
            // deselect() -> change sprite for finished levels
            for (int i=1; i<8; ++i)
            {
                levelComplete[i] = swimData[i-1];
                // deselect() all LevelChangers
                levels[i].deselect();
            }
        }
        else // new save file; all `False`
        {
            string[] lines = {"False","False","False","False","False","False","False"};
            File.WriteAllLines("swimData.txt", lines);
        }

        // hard-coded pArray
        pArray[0] = new List<int>(); // dummy
        pArray[1] = new List<int> {4};
        pArray[2] = new List<int> {7,6};
        pArray[3] = new List<int> {7,6,5,4,3,2,1};
        pArray[4] = new List<int> {6,4,2};
        pArray[5] = new List<int> {3,2,1};
        pArray[6] = new List<int> {6,5,4,1};
        pArray[7] = new List<int> {5,3,2};
        // hard-coded pblocks
        pblocks[0] = new List<string>(); // dummy
        pblocks[1] = new List<string> {"if","wait","wait","wait","flip","flip"};
        pblocks[2] = new List<string> {"wait","wait","flip","flip","flip","loop"};
        pblocks[3] = new List<string> {"if","wait","wait","flip","flip","flip","loop"};
        pblocks[4] = new List<string> {"if","wait","wait","wait","flip","flip","flip","loop"};
        pblocks[5] = new List<string> {"if","wait","wait","flip","flip","flip","loop","loop"};
        pblocks[6] = new List<string> {"if","if","wait","flip","flip","loop","loop"};
        pblocks[7] = new List<string> {"if","wait","wait","flip","flip","flip","loop"};
    }

    /*
        changeLevel ()
        public method, can be called by any object in the scene
        called when user presses level button (or run button apparently)
        change currently selected level, update UI accordingly
    */
    public void changeLevel (int newLevel, bool isPre)
    {
        // stop current animations, coroutine functions
        youAnimator.SetTrigger("resetT");
        partnerAnimator.SetTrigger("resetT");
        StopAllCoroutines();

        // hide all indicators
        foreach (Transform child in indicators)
            child.gameObject.SetActive(false);

        // selecting new level:
        if (isPre)
        {
            // deselect previous level (if applicable)
            if (levelSelected != 0)
                levels[levelSelected].deselect();

            // select new level
            levelSelected = newLevel;
            levels[levelSelected].select();

            // hide all stars, reset `you` Animator
            for (int i=1; i<8; ++i)
            {
                stars[i].enabled = false;
                youAnimator.SetBool($"{i}", false);
            }

            // clear vertical, grid panels O(n)
            foreach (Transform child in verticalTransform)
                Destroy(child.gameObject);
            foreach (Transform child in gridTransform)
                Destroy(child.gameObject);

            // by level: fill grid panel with pblocks
            // adding int (ex: 1) to differentiate pblocks
            int n=0;
            foreach (string pblock in pblocks[newLevel])
            {
                // create clone from `pblockHolder`
                GameObject clone = Instantiate(pblockHolder.Find(pblock).gameObject);
                // set parent to `gridpanel`
                clone.transform.SetParent(gridTransform);
                // reset name, scale
                clone.name = pblock + " " + n;
                clone.transform.localScale = Vector3.one;
                ++n;
            }

            // by level: display stars
            foreach (int star in pArray[levelSelected])
                stars[star].enabled = true;
            
            Debug.Log("Changed level to " + levelSelected);
        }

        // start coroutine: play diving animations
        StartCoroutine(PlayAnimationAndWait(isPre));
    }

    /*
        PlayAnimationAndWait ()
        @param isPre: predive

        set animation parameters then run
    */
    IEnumerator PlayAnimationAndWait(bool isPre)
    {
        // set `partner` animation and dive
        partnerAnimator.SetInteger("levelSelected", levelSelected);
        partnerAnimator.SetTrigger("diveT");

        // prepare `you` animation
        if (!isPre)
        {
            Debug.Log("preparing you dive");

            // clear `uArray` & `you` Animator (when do `you` flip)
            uArray.Clear();
            for (int i0=1; i0<8; ++i0)
                youAnimator.SetBool($"{i0}", false);

            // extract name, condition(if), reps(loop) as (string, int, int) from blocks in `VerticalPanel` into `blocks` list
            List<(string, string, int)> blocks = new List<(string, string, int)>();
            foreach (Transform child in verticalTransform)
            {
                // extract block name, index
                string name = child.name;
                string nam = name.Substring(0,name.IndexOf(" ")); // extract command
                string cond = "";
                int repeats = 0;

                // `loop`: extract repeats
                if (nam == "loop")
                {
                    repeats = child.Find("Dropdown").GetComponent<TMP_Dropdown>().value;
                    Debug.Log($"loop reps: {repeats}");
                }
                // if: get condition
                else if (nam == "if")
                {
                    // which condition are we checking (0: height, 1: color)
                    // by condition: check corresponding dropdown
                    int c = child.Find("Dropdown").GetComponent<TMP_Dropdown>().value;
                    if (c == 0)
                        cond = "0" + child.Find("height").GetComponent<TMP_Dropdown>().value;
                    else
                        cond = "1" + child.Find("color").GetComponent<TMP_Dropdown>().value;
                }

                // add to list
                blocks.Add((nam, cond, repeats));
            }

            // debugging
            foreach (var block in blocks)
                Debug.Log($"{block.Item1};{block.Item2};{block.Item3}");

            // while there are `blocks` and `you` height > 0
            // i: index
            // height: height
            // reps: list of ints, how many repeats left for this loop
            // top: list of ints, index of block after left bracket
            // indicator: array of lists of ints, per height, which block(s) are evaluated? dummy index 0
            int i=0, height=7;
            List<int> reps = new List<int>();
            List<int> top = new List<int>();
            List<int>[] indicator = new List<int>[8]; // initialize lists as empty lists
            for (int x=0; x<8; ++x) { indicator[x] = new List<int>(); }
            while (i<blocks.Count && height>0)
            {
                // note: i always increments at the end
                switch (blocks[i].Item1)
                {
                    // flip: add to indicator, set animation, add to uArray, decrease height
                    case "flip":
                        indicator[height].Add(i);
                        youAnimator.SetBool(""+height, true);
                        uArray.Add(height);
                        --height;
                        break;
                    // wait: add to indicator, decrease height
                    case "wait":
                        indicator[height].Add(i);
                        --height;
                        break;
                    // loop: add this and next block to indicator, add to `reps`, move i, add to `top`
                    case "loop":
                        indicator[height].Add(i);
                        // count reps
                        reps.Insert(0, blocks[i].Item3);
                        ++i;
                        top.Insert(0, i);
                        break;
                    // if: add to indicator, check condition, true: i past left bracket (2), add to 'reps' and 'top'
                    case "if":
                        indicator[height].Add(i);
                        bool match = false;
                        // height
                        if (blocks[i].Item2.Substring(0,1) == "0")
                        {
                            // match if `height` matches dropdown height
                            match = (height == 1+int.Parse(blocks[i].Item2.Substring(1)));
                        }
                        else // color
                        {
                            switch (blocks[i].Item2.Substring(1))
                            {
                                case "0": // green 7
                                    match = (height == 7);
                                    break;
                                case "1": // black 6,4
                                    match = (height == 6 || height == 4);
                                    break;
                                case "2": // blue 5,3,2
                                    match = (height == 5 || height == 3 || height == 2);
                                    break;
                                case "3": // red 1
                                    match = (height == 1);
                                    break;
                            }
                        }

                        // condition match: move i 2 & keep height, else keep i & decrease height
                        if (match)
                        {
                            ++i;
                            reps.Insert(0,0);
                            top.Insert(0,0);
                        }
                        else
                        {
                            --i;
                            --height;
                        }
                        break;
                    // }: end of loop or if
                    case "}":
                        Debug.Log("}" + reps[0] + top[0]);
                        // if there are reps left (loop), move i to `top` and decrement `reps`
                        if (reps[0] > 0)
                        {
                            i = top[0];
                            --reps[0];
                        }
                        else // no more reps, or if (0), remove
                        {
                            reps.RemoveAt(0);
                            top.RemoveAt(0);
                        }
                        break;
                }
                ++i;
            }
            // start `you` dive
            youAnimator.SetTrigger("diveT");

            // yield: wait for `you` then show indicators
            for (int k=7; k>0; --k)
            {
                Debug.Log($"now waiting for youdive {k}");
                // yield return new WaitUntil(() => partnerAnimator.GetCurrentAnimatorStateInfo(0).IsName($"dive to {k}"));
                yield return new WaitUntil(() => youAnimator.GetCurrentAnimatorStateInfo(0).IsName($"youdive {k}"));

                // first hide all indicators
                foreach (Transform child in indicators)
                    child.gameObject.SetActive(false);

                // show the indicators for the given height
                foreach (int index in indicator[k])
                {
                    Debug.Log($"height {k} index {index}");
                    indicators.Find(""+index).gameObject.SetActive(true);
                }

                // todo after dive, hide the indicators again
                // foreach (Transform child in indicators)
                //     child.gameObject.SetActive(false);
            }
            // yield return new WaitUntil(() => partnerAnimator.GetCurrentAnimatorStateInfo(0).IsName("dive to 7"));
            // yield return new WaitUntil(() => youAnimator.GetCurrentAnimatorStateInfo(0).IsName("youdive 7"));

            // yields wait for both dives to finish
            yield return new WaitUntil(() => partnerAnimator.GetCurrentAnimatorStateInfo(0).IsName("blueidle"));
            yield return new WaitUntil(() => youAnimator.GetCurrentAnimatorStateInfo(0).IsName("youidle"));

            // dives (list) match -> complete level
            if (uArray.SequenceEqual(pArray[levelSelected]))
            {
                levelComplete[levelSelected] = true;
                levels[levelSelected].deselect();
                levelSelected = 0;

                // todo say good job

                // SAVE FILE MANAGEMENT
                // convert bool array to string array, skipping the first value
                string[] lines = levelComplete.Skip(1).Take(7).Select(b => b.ToString()).ToArray();
                File.WriteAllLines("swimData.txt", lines);

                // check if all levels complete
                allLevelsComplete = (levelComplete[1]&&levelComplete[2]&&levelComplete[3]&&levelComplete[4]&&levelComplete[5]&&levelComplete[6]&&levelComplete[7]);
            }
            // todo if all levels are done, say good job again
        }
        else // pre-dive: wait for partner to finish dive
        {
            yield return new WaitUntil(() => partnerAnimator.GetCurrentAnimatorStateInfo(0).IsName("dive to 7"));
            yield return new WaitUntil(() => partnerAnimator.GetCurrentAnimatorStateInfo(0).IsName("blueidle"));
        }

        Debug.Log( (isPre ? "pre" : "") + "dive finished");
    }

    // called by run button
    // start coroutine as a real dive
    public void dive ()
    {
        changeLevel(levelSelected, false);
    }

    // called by pressing pblocks in `GridPanel`
    // todo change colors
    public void addBlock ()
    {
        Button btn = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        string name = btn.name; // extract GameObject name
        string nam = name.Substring(0,name.IndexOf(" ")); // extract command
        string e = name.Substring(name.IndexOf(" ")+1); // extract index

        // disable button
        btn.interactable = false;

        // create clone of `block` from blockHolder
        GameObject clone = Instantiate(blockHolder.Find(nam).gameObject);
        // set parent: VerticalPanel
        clone.transform.SetParent(verticalTransform);
        // set name, scale
        clone.name = name;
        clone.transform.localScale = Vector3.one;

        // if, loop: add bracket blocks
        if (nam == "if" || nam == "loop")
        {
            Debug.Log(name);
            GameObject clone2 = Instantiate(blockHolder.Find("{").gameObject);
            clone2.transform.SetParent(verticalTransform);
            clone2.name = "{ " + e;
            clone2.transform.localScale = Vector3.one;

            GameObject clone3 = Instantiate(blockHolder.Find("}").gameObject);
            clone3.transform.SetParent(verticalTransform);
            clone3.name = "} " + e;
            clone3.transform.localScale = Vector3.one;
        }

        Debug.Log($"added {name}");
    }

    // x, up, down
    public void moveBlock (string action)
    {
        Transform btn = EventSystem.current.currentSelectedGameObject.transform;
        Transform block = btn.parent;
        string name = block.name;
        string nam = name.Substring(0,name.IndexOf(" ")); // extract command
        string e = name.Substring(name.IndexOf(" ")+1); // extract index

        // siblingIndex, up/down
        int sI = block.GetSiblingIndex();

        // action
        switch (action)
        {
            // remove block
            case "x":
                // enable the pblock
                GameObject pblock = gridTransform.Find(name).gameObject;
                pblock.GetComponent<Button>().interactable = true;

                // if, loop: remove brackets
                if (nam == "if" || nam == "loop")
                {
                    Destroy(verticalTransform.Find("} " + e).gameObject);
                    Destroy(verticalTransform.Find("{ " + e).gameObject);
                }

                // remove block
                Destroy(block.gameObject);
                break;

            case "up":
            {
                // do nothing if we're highest block
                if (sI == 0) return;

                // get name, command of above block
                string aname = verticalTransform.GetChild(sI-1).name;
                string anam = aname.Substring(0,aname.IndexOf(" "));

                // above is "{" move up 2 steps, otherwise 1
                int steps = (anam == "{") ? 2 : 1;

                // we are if/loop, move all children up
                if (nam == "if" || nam == "loop")
                {
                    int rI = verticalTransform.Find("} " + e).GetSiblingIndex();
                    int difference = 1 + rI - sI; // ex: 1 + 3 - 1 = 3 (3 moves)

                    for (int i=0; i<difference; ++i)
                        verticalTransform.GetChild(rI).SetSiblingIndex(sI-steps);
                }
                else // we are normal block, move up steps 
                    block.SetSiblingIndex(sI-steps);

                break;
            }

            case "down":
            {
                // do nothing if we're lowest block
                // rI: index of right bracket
                // bname: name of block under us
                // if `if`, `loop` then bname is below our }
                int rI=0;
                string bname="";
                if (nam == "if" || nam == "loop")
                {
                    rI = verticalTransform.Find("} " + e).GetSiblingIndex();
                    if (rI == verticalTransform.childCount-1) return;

                    bname = verticalTransform.GetChild(rI+1).name;
                }
                else
                {
                    if (sI == verticalTransform.childCount-1) return;

                    bname = verticalTransform.GetChild(sI+1).name;
                }
                string bnam = bname.Substring(0,bname.IndexOf(" ")); // extract command

                // below is "if/loop" move each block down 2, otherwise 1
                int steps = (bnam == "if" || bnam == "loop") ? 2 : 1;

                // we are if/loop, move all children down
                if (nam == "if" || nam == "loop")
                {
                    int difference = 1 + rI - sI; // ex: 1 + 3 - 1 = 3 (3 moves)

                    for (int i=0; i<difference; ++i)
                        verticalTransform.GetChild(sI).SetSiblingIndex(rI+steps);
                }
                else // we are normal block, move down steps 
                    block.SetSiblingIndex(sI+steps);

                break;
            }
        }
    }

    // every frame
    void Update ()
    {
    }
}