using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ウィンドウをドラッグして移動する
/// </summary>
public class WindowMoveDragger : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GameObject clickBlocker = null;
    [SerializeField] private RectTransform window = null;
    [SerializeField] private RectTransform maxAreaRect = null;

    private Vector2 startWindowPosition_ = Vector2.zero;
    private Vector2 startTouchPosition_ = Vector2.zero;
    private Vector2 minTouchPosition_ = Vector2.zero;
    private Vector2 maxTouchPosition_ = Vector2.zero;

    void Start()
    {
        if (clickBlocker != null) { clickBlocker.SetActive(false); }
    }

    /// <summary>
    /// クリックされた
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // ドラッグ可能範囲設定
        Vector2 currentTouchPosition = eventData.position;
        Vector2 pivot = window.pivot;
        SetPivotWithKeepingPosition(window, Vector2.one);
        minTouchPosition_.x = currentTouchPosition.x - (maxAreaRect.rect.width - window.rect.width + window.anchoredPosition.x) * maxAreaRect.lossyScale.x;
        minTouchPosition_.y = currentTouchPosition.y - (maxAreaRect.rect.height - window.rect.height + window.anchoredPosition.y) * maxAreaRect.lossyScale.y;
        maxTouchPosition_.x = currentTouchPosition.x - window.anchoredPosition.x * maxAreaRect.lossyScale.x;
        maxTouchPosition_.y = currentTouchPosition.y - (window.anchoredPosition.y * maxAreaRect.lossyScale.y);
        SetPivotWithKeepingPosition(window, pivot);

        // タッチ座標保持
        startTouchPosition_ = currentTouchPosition;
        startWindowPosition_ = window.position;

        // 表示切替
        if (clickBlocker != null) { clickBlocker.SetActive(true); }
    }

    /// <summary>
    /// クリックが離れた
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // 表示切替
        if (clickBlocker != null) { clickBlocker.SetActive(false); }
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // ドラッグ範囲調整
        Vector2 currentTouchPosition = eventData.position;
        if (currentTouchPosition.x < minTouchPosition_.x) { currentTouchPosition.x = minTouchPosition_.x; }
        if (currentTouchPosition.y < minTouchPosition_.y) { currentTouchPosition.y = minTouchPosition_.y; }
        if (currentTouchPosition.x > maxTouchPosition_.x) { currentTouchPosition.x = maxTouchPosition_.x; }
        if (currentTouchPosition.y > maxTouchPosition_.y) { currentTouchPosition.y = maxTouchPosition_.y; }

        // ウィンドウ移動
        Vector2 diffPoint = currentTouchPosition - startTouchPosition_;
        //diffPoint.x /= maxAreaRect.lossyScale.x;
        //diffPoint.y /= maxAreaRect.lossyScale.y;
        Vector2 position = startWindowPosition_ + diffPoint;
        window.position = position;
    }

    /// <summary>
    /// 座標を保ったままPivotを変更する
    /// </summary>
    /// <param name="rectTransform">自身の参照</param>
    /// <param name="targetPivot">変更先のPivot座標</param>
    private void SetPivotWithKeepingPosition(RectTransform rectTransform, Vector2 targetPivot)
    {
        var diffPivot = targetPivot - rectTransform.pivot;
        rectTransform.pivot = targetPivot;
        var diffPos = new Vector2(rectTransform.sizeDelta.x * diffPivot.x, rectTransform.sizeDelta.y * diffPivot.y);
        rectTransform.anchoredPosition += diffPos;
    }
}
