using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenBehaviourScript : MonoBehaviour
{
    [SerializeField] private bool isAktiveOnStart = true;
    [SerializeField] private bool isAktive;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float moveSpeed = 5f;
    private int _currentWaypoint;
    
    
    
    // Start is called before the first frame update
    private void Start()
    {
        if (waypoints.Count <= 0) return;
        _currentWaypoint = 0;
        isAktive = isAktiveOnStart;
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        if(!isAktive) return;
        
        transform.position = Vector3.MoveTowards(transform.position, waypoints[_currentWaypoint].transform.position,
            (moveSpeed * Time.deltaTime));
    
        if (Vector3.Distance(waypoints[_currentWaypoint].transform.position, transform.position) <= 0.1f)
        {
            _currentWaypoint++;
        }
    
        if (_currentWaypoint != waypoints.Count) return;
        waypoints.Reverse();
        _currentWaypoint = 0;
    }

    public void SetAktive()
    {
        isAktive = true;
    }

    public void SetDiaktive()
    {
        isAktive = false;
    }
    
    public void ToggleAktive()
    {
        isAktive = !isAktive;
    }
}
