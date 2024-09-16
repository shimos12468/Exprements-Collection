using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;


[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path Path
    {
        get
        {
            return creator.path;
        }
    }

    const float segmentSelectDistanceThreshold = 0.1f;
    int selectedSegmentIndex = -1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create Path"))
        {
            Undo.RecordObject(creator, "Create A Path");
            creator.CreatePath();
        }

        bool isClosed = GUILayout.Toggle(Path.IsClosed, "Closed");
        if (isClosed!=Path.IsClosed)
        {
            Undo.RecordObject(creator, "Toggle Clossed");
            Path.IsClosed = isClosed;
        }


        bool autoSetControlPoints = GUILayout.Toggle(Path.AutoSetControlPoints, "Auto Set Control Points");
        if (autoSetControlPoints != Path.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle Auto Set Controls");
            Path.AutoSetControlPoints = autoSetControlPoints;
        }

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }




    }
    private void OnSceneGUI()
    {
        Input();
        Draw();
    }


    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0&& guiEvent.shift)
        {
            if (selectedSegmentIndex != -1)
            {
                Undo.RecordObject(creator, "Split Segment");
                Path.SplitSegment(mousePos,selectedSegmentIndex);
            }
            else if (!Path.isClosed)
            {
                Undo.RecordObject(creator, "Add Segment");
                Path.AddSegment(mousePos);
            }
            
        }
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1&&guiEvent.shift)
        {
            float minDistToAnchor = creator.anchorDiameter*.5f;
            int closestAnchorIndex = -1;

            for(int i = 0; i < Path.NumOfPoints; i += 3)
            {
                float dist = Vector2.Distance(mousePos, Path[i]);

                if (dist <minDistToAnchor)
                {
                    minDistToAnchor = dist;
                    closestAnchorIndex = i;
                }
            }
            if (closestAnchorIndex != -1)
            {
                Undo.RecordObject(creator, "Delete Segment");
                Path.DeleteSegment(closestAnchorIndex);
            }
        }

        if(guiEvent.type == EventType.MouseMove)
        {
            float minDist = segmentSelectDistanceThreshold;
            int newSelectedSegmentIndex = -1;
            for(int i = 0; i < Path.NumOfSegments; i++)
            {
                Vector2[] points = Path.GetPointsInSegment(i);
                float dist = HandleUtility.DistancePointBezier(mousePos, points[0], points[3], points[1], points[2]);

                if (dist < minDist)
                {
                    minDist = dist;
                    newSelectedSegmentIndex = i;
                }

            }

            if (newSelectedSegmentIndex != selectedSegmentIndex)
            {
                selectedSegmentIndex = newSelectedSegmentIndex;
                HandleUtility.Repaint();
            }
        }


    }
    void Draw()
    {

        for(int i = 0; i < Path.NumOfSegments; i++)
        {
            Vector2[]points = Path.GetPointsInSegment(i);

            if (creator.displayControlPoints)
            {
                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
            }
            Handles.color = i==selectedSegmentIndex&&Event.current.shift? creator.selectedSegmentCol : creator.segmentCol;
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Handles.color, null, 2f);
        }

        for(int i = 0; i < Path.NumOfPoints; i++)
        {

            if (i % 3 == 0 || creator.displayControlPoints)
            {
                float handleSize;

                if (i % 3 == 0)
                {
                    Handles.color = creator.anchorCol;
                    handleSize = creator.anchorDiameter;
                }
                else
                {
                    Handles.color = creator.controlCol;
                    handleSize = creator.controlDiameter;
                }

                Vector2 newPos = Handles.FreeMoveHandle(Path[i], handleSize, Vector2.zero, Handles.CylinderHandleCap);
                if (Path[i] != newPos)
                {
                    Path.MovePoint(i, newPos);
                }
            }
        }

    }

    private void OnEnable()
    {
        creator = (PathCreator)target;
        if (Path == null)
        {
            creator.CreatePath();
        }
    }

}
