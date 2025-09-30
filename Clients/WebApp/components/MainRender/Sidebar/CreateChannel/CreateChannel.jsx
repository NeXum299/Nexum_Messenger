export default function CreateChannel({ onClose }) {
    return (
        <div>
            <div>
                <button onClick={onClose}>Закрыть</button>
                <form>
                    <div>
                        <label>Название канала:</label>
                        <input type="text" required />
                    </div>
                    <div>
                        <label>Описание:</label>
                        <input type="text" required />
                    </div>
                </form>
            </div>
        </div>
    );
}
