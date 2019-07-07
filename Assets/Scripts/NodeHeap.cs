using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHeap
{
    int[] locationMap;
    VoronoiNode[] nodeHeap;
    int numNodes, m, f, l, r, p;

    public void Start() {
        numNodes = m = f = l = r = p = 0;
    }

    public void InitHeap(VoronoiNode[] l) {
        int n = l.Length;
        locationMap = new int[n];
        nodeHeap = l;
        for (int i = 0; i < n; i++) {
            locationMap[i] = i;
        }
    }

    public bool IsEmpty() {
        return numNodes == 0;
    }

    void PrintHeap() {
        string res = "full heap: ";
        for (int i = 0; i < numNodes; i++) {
            res = res + nodeHeap[i].Id + ": " + nodeHeap[i].Cost + ", ";
        }
        Debug.Log(res);
    }

    public void Push(int i) {
        string res = "";
        for (int j = 0; j < 3; j++) {
            res = res + nodeHeap[locationMap[i]].GetNeighbors()[j] + ", ";
        }
        Debug.Log(i + " pushed, with nbrs " + res);
        nodeHeap[locationMap[i]].Open = true;
        SwapNodes(locationMap[i], numNodes);
        numNodes++;
        TrickleUp(numNodes - 1);
        PrintHeap();
    }

    public void UpdateNode(int i) {
        Debug.Log(i + " updated");
        TrickleUp(locationMap[i]);
        PrintHeap();
    }

    public VoronoiNode Pop() {
        string res = "";
        for (int j = 0; j < 3; j++) {
            res = res + nodeHeap[0].GetNeighbors()[j] + ", ";
        }
        Debug.Log(nodeHeap[0].Id + " popped, with nbrs " + res);
        nodeHeap[0].Open = false;
        nodeHeap[0].Closed = true;
        //could test for size 0 here, but wont for speed
        VoronoiNode top = nodeHeap[0];
        numNodes--;
        SwapNodes(0, numNodes);
        TrickleDown(0);
        PrintHeap();
        return top;
    }

    public void ResetHeap() {
        numNodes = 0;
        for (int i = 0; i < numNodes; i++) {
            nodeHeap[i].Reset();
        }
    }

    void SwapNodes(int i, int j) {
        VoronoiNode tNode = nodeHeap[i];
        nodeHeap[i] = nodeHeap[j];
        nodeHeap[j] = tNode;
        locationMap[nodeHeap[j].Id] = j;
        locationMap[nodeHeap[i].Id] = i;
    }

    void TrickleDown(int i) {
        SetTD(i);
        while (NodeLT(nodeHeap[m], nodeHeap[f])) {
            SwapNodes(f, m);
            SetTD(m);
        }
    }

    void SetTD(int i) {
        f = i;
        l = (i << 1) + 1;
        r = l + 1;
        if (r < numNodes) {
            m = NodeLT(nodeHeap[l], nodeHeap[r]) ? l : r;
        } else if (l >= numNodes) {
            m = f;
        } else {
            m = l;
        }
    }

    void TrickleUp(int i) {
        SetTU(i);
        if (f == 0) return;
        while (NodeLT(nodeHeap[f], nodeHeap[p])) {
            SwapNodes(f, p);
            SetTU(p);
            if (f == 0) return;
        }
    }

    void SetTU(int i) {
        f = i;
        p = (i - 1) >> 1;
    }

    bool NodeLT(VoronoiNode a, VoronoiNode b) {
        return (a.Cost < b.Cost);
    }

    public void SetGiven(int i, int p, float g) {
        nodeHeap[locationMap[i]].Parent = p;
        nodeHeap[locationMap[i]].Given = g;
        nodeHeap[locationMap[i]].Cost = nodeHeap[locationMap[i]].Given + nodeHeap[locationMap[i]].Hueristic;
    }

    public void SetHeuristic(int i, float h) {
        nodeHeap[locationMap[i]].Hueristic = h;
    }

    public VoronoiNode GetNode(int i) {
        return nodeHeap[locationMap[i]];
    }

}
