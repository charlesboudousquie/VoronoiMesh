using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeHeap
{
    int[] locationMap;
    VoronoiNode[] nodeHeap;
    VoronoiNode nodeStart;
    int numNodes, m, f, l, r, p;

    public void Start() {
        locationMap = new int[4096];
        nodeHeap = new VoronoiNode[4096];
    }

    public void InitHeap(VoronoiNode[] l) {
        nodeStart = l[0];
        for (int i = 0; i < 4096; i++) {
            locationMap[i] = i;
            nodeHeap[i] = l[i];
        }
    }

    public bool IsEmpty() {
        return numNodes == 0;
    }

    public void Push(int i) {
        SwapNodes(locationMap[i], numNodes);
        numNodes++;
        TrickleUp(numNodes - 1);
    }

    public void UpdateNode(int i) {
        TrickleUp(locationMap[i]);
    }

    public VoronoiNode Pop() {
        //could test for size 0 here, but wont for speed
        VoronoiNode top = nodeHeap[0];
        numNodes--;
        SwapNodes(0, numNodes);
        TrickleDown(0);
        return top;
    }

    public void ResetHeap() {
        numNodes = 0;
    }

    void SwapNodes(int i, int j) {
        VoronoiNode tNode = nodeHeap[i];
        nodeHeap[i] = nodeHeap[j];
        nodeHeap[j] = tNode;
        //locationMap[nodeHeap[j].Id] = j;
        //locationMap[nodeHeap[i].Id] = i;
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
        return false;
        //return (a.Cost < b.Cost);
    }

}
