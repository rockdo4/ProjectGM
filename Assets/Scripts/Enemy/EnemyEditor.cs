using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyAI))]
public class EnemyAIEditor : Editor
{
    private string newPatternName = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyAI script = (EnemyAI)target;

        // 새 패턴 이름 입력 필드
        newPatternName = EditorGUILayout.TextField("New Pattern Name", newPatternName);

        // 저장 버튼
        if (GUILayout.Button("Save Current Pattern"))
        {
            SaveAttackPattern(script, newPatternName, script.attackGrid);
        }

        // 저장된 패턴 목록 표시 (옵션)
        if (script.savedPatterns.Count > 0)
        {
            EditorGUILayout.LabelField("Saved Patterns:");
            foreach (var pattern in script.savedPatterns)
            {
                EditorGUILayout.LabelField(pattern.patternName);
            }
        }
    }

    private void SaveAttackPattern(EnemyAI script, string patternName, bool[] patternData)
    {
        if (string.IsNullOrWhiteSpace(patternName))
        {
            Debug.LogError("Pattern name is empty");
            return;
        }

        // 동일한 이름의 패턴 찾기
        var existingPattern = script.savedPatterns.Find(p => p.patternName == patternName);

        if (existingPattern != null)
        {
            // 이미 존재하는 패턴이면 데이터 업데이트
            existingPattern.pattern = patternData;
        }
        else
        {
            // 새로운 패턴이면 리스트에 추가
            script.savedPatterns.Add(new AttackPattern(patternName, patternData));
        }

        // 변경 사항 적용
        EditorUtility.SetDirty(script);
    }
}
