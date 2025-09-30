import { useState } from 'react';
import Sidebar from './Sidebar/Sidebar.jsx';
import MessageInput from './MessageInput/MessageInput.jsx';
import Chatheader from './ChatHeader/ChatHeader.jsx';
import CharArea from './ChatArea/ChatArea.jsx';
import ItemList from './ItemList/ItemList.jsx';
import ListHeader from './ListHeader/ListHeader.jsx';
import styles from './MainRender.module.css';
import '../../main.css';

export default function MainRender() {
    const [selectedFriend, setSelectedFriend] = useState(null);
    const [selectedGroup, setSelectedGroup] = useState(null);

    const handleSelectItem = (item) => {
        setSelectedFriend(null);
        setSelectedGroup(null);

        switch (item.type) {
            case 'friend':
                setSelectedFriend(item);
                break;
            case 'group':
                setSelectedGroup(item);
                break;
            default:
                console.warn('Unknown item type:', item.type);
        }
    }

    const handleCloseItem = () => {
        setSelectedFriend(null);
        setSelectedGroup(null);
    }

    const activeItem = selectedFriend || selectedGroup;

    return (
        <div className={styles["main-container"]}>
            <div className={styles["second-column"]}>
                <div className={styles["list-header"]}>
                    <ListHeader />
                </div>

                <div className={styles["item-list"]}>
                    <ItemList
                        selectedFriend={selectedFriend}
                        selectedGroup={selectedGroup}
                        onSelect={handleSelectItem} />
                </div>
            </div>

            <div className={styles["third-column"]}>
                {activeItem && (
                    <>
                        <div className={styles["chat-header-area"]}>
                            <Chatheader
                                selectedFriend={selectedFriend}
                                selectedGroup={selectedGroup}
                                onCloseItem={handleCloseItem} />
                            <CharArea
                                selectedItem={activeItem}
                                onCloseItem={handleCloseItem} />
                        </div>

                        <div className={styles["message-input"]}>
                            <MessageInput 
                                selectedFriend={selectedFriend}
                                selectedGroup={selectedGroup} />
                        </div>
                    </>
                )}
            </div>
        </div>
    );
}
