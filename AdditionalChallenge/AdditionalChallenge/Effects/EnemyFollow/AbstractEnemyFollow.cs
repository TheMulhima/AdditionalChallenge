namespace AdditionalChallenge.Effects.EnemyFollow;

public abstract class AbstractEnemyFollow:AbstractEffects
{
    public void Update()
    {
        if (!IsEnabled)
        {
            if (Enemy != null)
            {
                Destroy(Enemy);
            }
            return;
        }
        if (HeroController.instance == null)
        {
            if (Enemy != null)
            {
                Destroy(Enemy);
            }
            return;
        }
        
        if (Enemy == null)
        {
            CreateEnemy();
        }
        
        Vector2 heroPos = HeroController.instance.transform.position;
        var xdiff = Mathf.Abs(Enemy.transform.position.x - heroPos.x);
        var ydiff = Mathf.Abs(Enemy.transform.position.y - heroPos.y);

        if (xdiff > 15 || ydiff > 15)
        {
            Enemy.transform.position = GetRandomPosForEnemy();
        }
    }

    protected Vector2 GetRandomPosForEnemy()
    {
        Vector2 heroPos = HeroController.instance.transform.position;
        var randomPoint = URandom.insideUnitCircle;
        randomPoint = new Vector2(randomPoint.x, Mathf.Abs(randomPoint.y)) * 10; 
        randomPoint += new Vector2(5, 5);
        return heroPos + randomPoint;
    }

    /// <summary>
    /// To be called in Create Enemy
    /// </summary>
    /// <param name="enemyPrefab"></param>
    protected void SetUpEnemy(GameObject enemyPrefab)
    {
        Enemy = UObject.Instantiate(enemyPrefab);
        Enemy.SetActive(true);
        Enemy.SetActiveChildren(true);
        Enemy.transform.position = GetRandomPosForEnemy();
        var hm = Enemy.GetComponent<HealthManager>();
        hm.hp = Int32.MaxValue;
        ReflectionHelper.SetField(hm,"enemyType", 6);//remove soul gain
    }

    protected GameObject Enemy;

    protected abstract void CreateEnemy();
    
    public override List<(PropertyInfo,AbstractEffects)> GetPropertiesToAdd()
    {
        return new List<(PropertyInfo, AbstractEffects)>()
        {
            (ReflectionHelper.GetPropertyInfo(typeof(AbstractEffects), nameof(IsEnabled)), this),
        };
    }

    public override void AddElementsToModMenu(Menu MenuRef)
    {
        MenuRef.AddElement(new HorizontalOption(ToggleName, ToggleDesc,
            new [] { "Enabled", "Disabled" },
            (i) =>
            {
                AdditionalChallenge.settings.Booleans[Key] = i == 0;
                AdditionalChallenge.Instance.MatchSettings();
            },
            () => AdditionalChallenge.settings.Booleans.ContainsKey(Key)
                ? AdditionalChallenge.settings.Booleans[Key] ? 0 : 1 
                : 1));
    }

    private string Key => MiscExtensions.GetKey(this, nameof(IsEnabled));
}