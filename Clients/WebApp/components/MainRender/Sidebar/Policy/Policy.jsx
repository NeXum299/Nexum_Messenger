export default function Policy({ onClose }) {
    return (
        <div>
            <div>
                <button onClick={onClose}>Закрыть</button>
                <h2>Настройки конфиденциальности</h2>
            </div>
        </div>
    );
}
