using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ウィンドウをドラッグして拡縮する
/// </summary>
public class WindowScaleDragger : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [System.Serializable]
    private enum HorizontalType
    {
        None,  // 操作不可
        Left,  // 左に伸ばす
        Right, // 右に伸ばす
    }

    [System.Serializable]
    private enum VerticalType
    {
        None,   // 操作不可
        Top,    // 上に伸ばす
        Bottom, // 下に伸ばす
    }

    [SerializeField] private GameObject clickBlocker = null;
    [SerializeField] private RectTransform frame = null;
    [SerializeField] private RectTransform window = null;
    [SerializeField] private RectTransform maxFrameSizeRect = null;
    [SerializeField] private Vector2 minFrameSize = new Vector2(300.0f, 400.0f);
    [SerializeField] private VerticalType verticalScaleType = VerticalType.Bottom;
    [SerializeField] private HorizontalType horizontalScaleType = HorizontalType.Left;

    private Vector2 startTouchPosition_ = Vector2.zero;
    private Vector2 startFrameSize_ = Vector2.zero;
    private Vector2 maxFrameSize_ = Vector2.zero;

    void Start()
    {
        if (clickBlocker != null) { clickBlocker.SetActive(false); }
        if (frame != null) { frame.gameObject.SetActive(false); }
    }

    /// <summary>
    /// 最大サイズにする
    /// </summary>
    public void SetMaxWindow()
    {
        Vector2 pivot = window.pivot;
        window.pivot = new Vector2(0.5f, 0.5f);
        window.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxFrameSizeRect.rect.width);
        window.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxFrameSizeRect.rect.height);
        window.position = maxFrameSizeRect.position;
        SetPivotWithKeepingPosition(window, pivot);
    }

    /// <summary>
    /// クリックされた
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        // ウィンドウのコピー
        frame.anchorMin = window.anchorMin;
        frame.anchorMax = window.anchorMax;
        frame.pivot = window.pivot;
        frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, window.rect.width);
        frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, window.rect.height);
        frame.position = window.position;

        // フレームのピボットを合わせる
        Vector2 pivot = Vector2.one;
        if (horizontalScaleType == HorizontalType.Right) { pivot.x = 0.0f; }
        if (verticalScaleType == VerticalType.Top) { pivot.y = 0.0f; }
        SetPivotWithKeepingPosition(frame, pivot);

        // 開始フレームサイズ保持
        startFrameSize_ = frame.rect.size;

        // ドラッグ可能な最大フレームサイズ設定
        maxFrameSize_ = maxFrameSizeRect.rect.size + frame.anchoredPosition;
        if (horizontalScaleType == HorizontalType.Right) { maxFrameSize_.x = maxFrameSizeRect.rect.size.x - frame.anchoredPosition.x; }
        if (verticalScaleType == VerticalType.Top) { maxFrameSize_.y = maxFrameSizeRect.rect.size.y - frame.anchoredPosition.y; }

        // タッチ座標保持
        startTouchPosition_ = eventData.position;

        // 表示切替
        if (clickBlocker != null) { clickBlocker.SetActive(true); }
        frame.gameObject.SetActive(true);
    }

    /// <summary>
    /// クリックが離れた
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        // フレームのコピー
        SetPivotWithKeepingPosition(frame, window.pivot);
        window.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frame.rect.width);
        window.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, frame.rect.height);
        window.position = frame.position;

        // 表示切替
        if (clickBlocker != null) { clickBlocker.SetActive(false); }
        frame.gameObject.SetActive(false);
    }

    /// <summary>
    /// ドラッグ中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        // フレームサイズ更新
        Vector2 currentTouchPosition = eventData.position;
        Vector2 diffPoint = startTouchPosition_ - currentTouchPosition;
        if (frame.lossyScale.x > 0.0f) { diffPoint.x /= frame.lossyScale.x; }
        if (frame.lossyScale.y > 0.0f) { diffPoint.y /= frame.lossyScale.y; }
        if (horizontalScaleType == HorizontalType.Right) { diffPoint.x *= -1.0f; }
        if (verticalScaleType == VerticalType.Top) { diffPoint.y *= -1.0f; }
        Vector2 frameSize = startFrameSize_ + diffPoint;
        if (frameSize.x < minFrameSize.x) { frameSize.x = minFrameSize.x; }
        if (frameSize.y < minFrameSize.y) { frameSize.y = minFrameSize.y; }
        if (frameSize.x > maxFrameSize_.x) { frameSize.x = maxFrameSize_.x; }
        if (frameSize.y > maxFrameSize_.y) { frameSize.y = maxFrameSize_.y; }
        if (horizontalScaleType != HorizontalType.None) { frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, frameSize.x); }
        if (verticalScaleType != VerticalType.None) { frame.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, frameSize.y); }
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
