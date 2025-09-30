import { useRef, useState } from 'react';
import styles from './GroupManagementMenu.module.css';
import AddMember from './GroupModals/AddMember/AddMember.jsx';
import EditGroup from './GroupModals/EditGroup/EditGroup.jsx';
import LeaveGroup from './GroupModals/LeaveGroup/LeaveGroup.jsx';
import DeleteGroup from './GroupModals/DeleteGroup/DeleteGroup.jsx';
import DeleteMember from './GroupModals/DeleteMember/DeleteMember.jsx';

export default function GroupManagementMenu({ isOpen, onClose, selectedItem, onCloseItem }) {
    const menuRef = useRef(null);
    const [activeModal, setActiveModal] = useState(null);

    const handleAddMemberClick = () => {
        setActiveModal('addMember');
    };

    const handleDeleteMemberClick = () => {
        setActiveModal('deleteMember');
    };

    const handleEditGroupClick = () => {
        setActiveModal('editGroup');
    };

    const handleLeaveGroupClick = () => {
        setActiveModal('leaveGroup');
    };
  
    const handleGroupDeleteClick = () => {
        setActiveModal('deleteGroup');
    };

    const handleModalClose = () => {
        setActiveModal(null);
        onClose();
    };

    const handleCloseItemClick = () => {
        onCloseItem();
        onClose();
    };

    if (!isOpen) return null;

    return (
        <>
            <div className={styles["menu-overlay"]}>
                <div 
                    ref={menuRef} 
                    className={`${styles["menu-container"]} ${isOpen ? styles["menu-open"] : ""}`}>

                    <button onClick={onClose}>X</button>

                    <div className={styles["menu-content"]}>
                        <button
                            className={styles["menu-item"]}
                            onClick={handleAddMemberClick}>
                            Добавить участника
                        </button>

                        <button
                            className={styles["menu-item"]}
                            onClick={handleDeleteMemberClick}>
                            Удалить участника
                        </button>
                    
                        <button
                            className={styles["menu-item"]}
                            onClick={handleEditGroupClick}>
                            Изменить группу
                        </button>
                    
                        <button
                            className={styles["menu-item"]}
                            onClick={handleLeaveGroupClick}>
                            Покинуть группу
                        </button>

                        <button
                            className={styles["menu-item"]}
                            onClick={handleGroupDeleteClick}>
                            Удалить группу
                        </button>
                        
                        <button
                            className={styles["menu-item"]}
                            onClick={handleCloseItemClick}>
                            Закрыть группу
                        </button>

                    </div>
                </div>
            </div>

            {activeModal === 'addMember' && (
                <AddMember
                    isOpen={true}
                    onClose={handleModalClose}
                    selectedItem={selectedItem} />
            )}

            {activeModal === 'deleteMember' && (
                <DeleteMember
                    isOpen={true}
                    onClose={handleModalClose}
                    selectedItem={selectedItem} />
            )}
            
            {activeModal === 'editGroup' && (
                <EditGroup
                    isOpen={true}
                    onClose={handleModalClose}
                    selectedItem={selectedItem} />
            )}
            
            {activeModal === 'leaveGroup' && (
                <LeaveGroup
                    isOpen={true}
                    onClose={handleModalClose}
                    selectedItem={selectedItem} />
            )}

            {activeModal === 'deleteGroup' && (
                <DeleteGroup
                    isOpen={true}
                    onClose={handleModalClose}
                    selectedItem={selectedItem} />
            )}
        </>
    );
}
